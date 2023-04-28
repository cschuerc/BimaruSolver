using System;
using System.Collections.Generic;
using System.Linq;
using Bimaru.Interface.Game;
using Bimaru.Interface.Solver;
using Bimaru.Interface.Utility;

namespace Bimaru.Solver
{
    public sealed class BimaruSolver : IBimaruSolver
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BimaruSolver"/> class.
        /// </summary>
        /// <param name="fieldChangedRules"> True means the number of solutions is counted.
        /// False means it stops after the first solution that is found. </param>
        /// <param name="solverRules"></param>
        /// <param name="trialRule"></param>
        /// <param name="gridBackup"></param>
        /// <param name="shallCountSolutions"></param>
        /// <exception cref="ArgumentException"></exception>
        public BimaruSolver(IEnumerable<IFieldValueChangedRule> fieldChangedRules,
            IEnumerable<ISolverRule> solverRules,
            ITrialAndErrorRule trialRule,
            IBackup<IBimaruGrid> gridBackup,
            bool shallCountSolutions = false)
        {
            ChangedRules = fieldChangedRules;
            SolverRules = solverRules;
            TrialRule = trialRule;
            GridBackup = gridBackup;
            ShallCountSolutions = shallCountSolutions;

            var isSolverCapableOfCounting = TrialRule != null &&
                                            TrialRule.AreTrialsDisjoint &&
                                            TrialRule.AreTrialsComplete;

            if (ShallCountSolutions && !isSolverCapableOfCounting)
            {
                throw new ArgumentException("This solver is not able to count the number of solutions.");
            }
        }

        #region Rules
        private List<IFieldValueChangedRule> changedRules;

        private IEnumerable<IFieldValueChangedRule> ChangedRules
        {
            get => changedRules;

            set
            {
                changedRules = new List<IFieldValueChangedRule>();

                if (value != null)
                {
                    changedRules.AddRange(value);
                }
            }
        }

        private List<ISolverRule> solverRules;

        private IEnumerable<ISolverRule> SolverRules
        {
            get => solverRules;

            set
            {
                solverRules = new List<ISolverRule>();

                if (value != null)
                {
                    solverRules.AddRange(value);
                }
            }
        }

        private ITrialAndErrorRule TrialRule
        {
            get;
        }
        #endregion

        private IBackup<IBimaruGrid> gridBackup;

        private IBackup<IBimaruGrid> GridBackup
        {
            get => gridBackup;

            set => gridBackup = value ?? throw new ArgumentNullException(nameof(value));
        }

        private bool ShallCountSolutions
        {
            get;
        }

        #region Solve
        public int Solve(IBimaruGame game)
        {
            var numberOfSolutions = SolveToClipboard(game, true);

            if (numberOfSolutions > 0)
            {
                GridBackup.RestoreFromClipboardTo(game.Grid);
            }

            return numberOfSolutions;
        }

        private int SolveToClipboard(IBimaruGame game, bool isFirstCall, FieldsToChange<BimaruValue> changes = null)
        {
            var numSolutions = 0;

            GridBackup.SetSavePoint(game.Grid);

            try
            {
                numSolutions = ApplyChangesAndRunRules(game, isFirstCall, changes);
            }
            catch (InvalidBimaruGameException)
            {
                // Some rules might violate the Bimaru constraints
                // => This cannot be a solution and we can ignore it.
            }
            finally
            {
                GridBackup.RestoreAndDeleteLastSavepoint(game.Grid);
            }

            return numSolutions;
        }

        private int ApplyChangesAndRunRules(IBimaruGame game, bool isFirstCall, FieldsToChange<BimaruValue> changes)
        {
            var hasChangedFieldValues = ApplyChangesAndRunNonTrialRules(game, isFirstCall, changes);
            if (!isFirstCall && !hasChangedFieldValues)
            {
                throw new InvalidOperationException(@"No field value has changed, which
                                                      could lead to an infinite recursion");
            }

            if (game.IsSolved)
            {
                GridBackup.CloneToClipboard(game.Grid);
                return 1;
            }

            return RunTrialRule(game);
        }

        private bool ApplyChangesAndRunNonTrialRules(IBimaruGame game, bool isFirstCall, FieldsToChange<BimaruValue> changes)
        {
            var hasChangedFieldValues = false;

            var unhandledChangedEvents = new Queue<FieldValueChangedEventArgs<BimaruValue>>();

            void ChangedEventHandler(object sender, FieldValueChangedEventArgs<BimaruValue> e)
            {
                CheckIsChangeValid(game, e);
                unhandledChangedEvents.Enqueue(e);
                hasChangedFieldValues = true;
            }

            if (isFirstCall)
            {
                FireInitialChangedEvents(game.Grid, ChangedEventHandler);
            }

            game.Grid.FieldValueChanged += ChangedEventHandler;

            try
            {
                game.Grid.ApplyFieldChanges(changes);
                HandleChangedEvents(game, unhandledChangedEvents);
                RunSolverRules(game, isFirstCall);
                HandleChangedEvents(game, unhandledChangedEvents);
            }
            finally
            {
                game.Grid.FieldValueChanged -= ChangedEventHandler;
            }

            return hasChangedFieldValues;
        }

        private static void CheckIsChangeValid(IBimaruGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            var newValue = game.Grid[e.Point];
            if (!game.IsValid || !e.OriginalValue.IsCompatibleChangeTo(newValue))
            {
                throw new InvalidFieldValueChangeException();
            }
        }

        private void FireInitialChangedEvents(IBimaruGrid grid, EventHandler<FieldValueChangedEventArgs<BimaruValue>> eventHandler)
        {
            foreach (var p in grid.AllPoints().Where(p => grid[p] != BimaruValue.UNDETERMINED))
            {
                eventHandler(this, new FieldValueChangedEventArgs<BimaruValue>(p, BimaruValue.UNDETERMINED));
            }
        }

        private void HandleChangedEvents(IBimaruGame game, Queue<FieldValueChangedEventArgs<BimaruValue>> unhandledChangedEvents)
        {
            while (unhandledChangedEvents.Count > 0)
            {
                var e = unhandledChangedEvents.Dequeue();
                foreach (var rule in ChangedRules)
                {
                    rule.FieldValueChanged(game, e);
                }
            }
        }

        private void RunSolverRules(IBimaruGame game, bool isFirstCall)
        {
            foreach (var rule in SolverRules.Where(rule => isFirstCall || !rule.ShallBeAppliedOnce))
            {
                rule.Solve(game);
            }
        }

        private int RunTrialRule(IBimaruGame game)
        {
            if (TrialRule == null)
            {
                return 0;
            }

            var numSolutions = 0;

            foreach (var changes in TrialRule.GetChangeTrials(game))
            {
                numSolutions += SolveToClipboard(game, false, changes);

                if (!ShallCountSolutions && numSolutions > 0)
                {
                    return numSolutions;
                }
            }

            return numSolutions;
        }
        #endregion
    }
}
