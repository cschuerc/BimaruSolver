using System;
using System.Linq;
using System.Collections.Generic;
using Utility;
using Bimaru.Interfaces;

namespace Bimaru.SolverUtil
{
    public class Solver : ISolver
    {
        /// <param name="shallCountSolutions">
        /// True means the number of solutions is counted.
        /// False means it stops after the first solution that is found.
        /// </param>
        public Solver(IEnumerable<IFieldValueChangedRule> fieldChangedRules,
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

            bool isSolverCapableOfCounting = TrialRule != null &&
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
            get
            {
                return changedRules;
            }

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
            get
            {
                return solverRules;
            }

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
            set;
        }
        #endregion

        private IBackup<IBimaruGrid> gridBackup;

        private IBackup<IBimaruGrid> GridBackup
        {
            get
            {
                return gridBackup;
            }

            set
            {
                gridBackup = value ?? throw new ArgumentNullException("GridBackup is null.");
            }
        }

        private bool ShallCountSolutions
        {
            get;
            set;
        }

        #region Solve
        public virtual int Solve(IGame game)
        {
            int numberOfSolutions = SolveToClipboard(game, true);

            if (numberOfSolutions > 0)
            {
                GridBackup.RestoreFromClipboardTo(game.Grid);
            }

            return numberOfSolutions;
        }

        private int SolveToClipboard(IGame game, bool isFirstCall, FieldsToChange<BimaruValue> changes = null)
        {
            int numSolutions = 0;

            GridBackup.SetSavePoint(game.Grid);

            try
            {
                numSolutions = ApplyChangesAndRunRules(game, isFirstCall, changes);
            }
            catch (InvalidBimaruGame)
            {
            }
            finally
            {
                GridBackup.RestoreAndDeleteLastSavepoint(game.Grid);
            }

            return numSolutions;
        }

        private int ApplyChangesAndRunRules(IGame game, bool isFirstCall, FieldsToChange<BimaruValue> changes)
        {
            bool hasChangedFieldValues = ApplyChangesAndRunNonTrialRules(game, isFirstCall, changes);
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

        private bool ApplyChangesAndRunNonTrialRules(IGame game, bool isFirstCall, FieldsToChange<BimaruValue> changes)
        {
            bool hasChangedFieldValues = false;

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

        private static void CheckIsChangeValid(IGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            BimaruValue newValue = game.Grid[e.Point];
            if (!game.IsValid || !e.OriginalValue.IsCompatibleChangeTo(newValue))
            {
                throw new InvalidFieldValueChange();
            }
        }

        private void FireInitialChangedEvents(IBimaruGrid grid, EventHandler<FieldValueChangedEventArgs<BimaruValue>> eventHandler)
        {
            foreach (GridPoint p in grid.AllPoints().Where(p => grid[p] != BimaruValue.UNDETERMINED))
            {
                eventHandler(this, new FieldValueChangedEventArgs<BimaruValue>(p, BimaruValue.UNDETERMINED));
            }
        }

        private void HandleChangedEvents(IGame game, Queue<FieldValueChangedEventArgs<BimaruValue>> unhandledChangedEvents)
        {
            while (unhandledChangedEvents.Count > 0)
            {
                var e = unhandledChangedEvents.Dequeue();
                foreach (IFieldValueChangedRule rule in ChangedRules)
                {
                    rule.FieldValueChanged(game, e);
                }
            }
        }

        private void RunSolverRules(IGame game, bool isFirstCall)
        {
            foreach (ISolverRule rule in SolverRules.Where(rule => isFirstCall || !rule.ShallBeAppliedOnce))
            {
                rule.Solve(game);
            }
        }

        private int RunTrialRule(IGame game)
        {
            if (TrialRule == null)
            {
                return 0;
            }

            int numSolutions = 0;

            foreach (FieldsToChange<BimaruValue> changes in TrialRule.GetChangeTrials(game))
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
