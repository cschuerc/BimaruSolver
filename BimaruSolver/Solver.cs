using System;
using System.Linq;
using System.Collections.Generic;
using Utility;
using BimaruInterfaces;

namespace BimaruSolver
{
    /// <summary>
    /// Standard implementation of ISolver
    /// </summary>
    public class Solver : ISolver
    {
        /// <summary>
        /// Creates a generic Bimaru solver.
        /// </summary>
        /// <param name="fieldChangedRules"> Solving rules triggered for every field change. </param>
        /// <param name="fullGridRules"> General solving rules without guessing. </param>
        /// <param name="trialRule"> Single rule that tries out different possibilities. </param>
        /// <param name="gridBackup"> Grid backup </param>
        /// <param name="shallCountSolutions"> True, if the solver shall count the number of solutions. </param>
        public Solver(IEnumerable<IFieldValueChangedRule> fieldChangedRules,
            IEnumerable<ISolverRule> fullGridRules,
            ITrialAndErrorRule trialRule,
            IBackup<IBimaruGrid> gridBackup,
            bool shallCountSolutions = false)
        {
            FieldChangedRules = fieldChangedRules;
            FullGridRules = fullGridRules;
            TrialRule = trialRule;
            GridBackup = gridBackup;
            ShallCountSolutions = shallCountSolutions;

            if (ShallCountSolutions && (TrialRule == null || !TrialRule.AreTrialsDisjoint))
            {
                throw new ArgumentException(@"This solver cannot count the number of
                                              solutions without a disjoint trial rule.");
            }
        }

        /// <inheritdoc/>
        public virtual int Solve(IGame game)
        {
            int numSolutions = RunRulesInSavepoint(game, true);

            if (numSolutions > 0)
            {
                // Restore the last solution from the clipboard
                GridBackup.RestoreFromClipboardTo(game.Grid);
            }

            return numSolutions;
        }

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

        #region Rules
        private List<IFieldValueChangedRule> _fieldChangedRules;

        /// <summary>
        /// Solving rules triggered after every field change.
        /// </summary>
        private IEnumerable<IFieldValueChangedRule> FieldChangedRules
        {
            get
            {
                return _fieldChangedRules;
            }

            set
            {
                _fieldChangedRules = new List<IFieldValueChangedRule>();

                if (value != null)
                {
                    _fieldChangedRules.AddRange(value);
                }
            }
        }

        private List<ISolverRule> _fullGridRules;

        /// <summary>
        /// General solving rules without guessing.
        /// </summary>
        private IEnumerable<ISolverRule> FullGridRules
        {
            get
            {
                return _fullGridRules;
            }

            set
            {
                _fullGridRules = new List<ISolverRule>();

                if (value != null)
                {
                    _fullGridRules.AddRange(value);
                }
            }
        }

        /// <summary>
        /// Single rule that tries out different possibilities
        /// </summary>
        private ITrialAndErrorRule TrialRule { get; set; }

        /// <summary>
        /// Whether the solver shall count the solutions or not.
        /// If not, it stops after having found the first solution.
        /// </summary>
        private bool ShallCountSolutions { get; set; }
        #endregion

        #region Run rules
        private int RunRulesInSavepoint(IGame game, bool isFirstCall, FieldsToChange<BimaruValue> changes = null)
        {
            int numSolutions = 0;

            GridBackup.SetSavePoint(game.Grid);

            try
            {
                numSolutions = RunRules(game, isFirstCall, changes);
            }
            catch (InvalidBimaruGame)
            {
            }
            catch (InvalidFieldValueChange)
            {

            }
            finally
            {
                GridBackup.RestoreAndDeleteLastSavepoint(game.Grid);
            }

            return numSolutions;
        }

        private int RunRules(IGame game, bool isFirstCall, FieldsToChange<BimaruValue> changes = null)
        {
            var unhandledChangedEvents = new Queue<FieldValueChangedEventArgs<BimaruValue>>();

            void FieldChangedHandler(object sender, FieldValueChangedEventArgs<BimaruValue> e)
            {
                CheckIsChangeValid(game, e);
                unhandledChangedEvents.Enqueue(e);
            }

            if (isFirstCall)
            {
                FireInitialFieldChangedEvents(game.Grid, FieldChangedHandler);
            }

            game.Grid.FieldValueChanged += FieldChangedHandler;

            try
            {
                if (!isFirstCall || changes != null)
                {
                    ApplyChanges(game, changes);
                }

                HandleChangedEvents(game, unhandledChangedEvents);
                RunFullGridRules(game, isFirstCall);
                HandleChangedEvents(game, unhandledChangedEvents);
            }
            finally
            {
                game.Grid.FieldValueChanged -= FieldChangedHandler;
            }

            return RunTrialAndErrorRule(game);
        }

        private static void CheckIsChangeValid(IGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            BimaruValue newValue = game.Grid[e.Point];
            if (!game.IsValid || !e.OriginalValue.IsCompatibleChangeTo(newValue))
            {
                throw new InvalidFieldValueChange();
            }
        }

        private void FireInitialFieldChangedEvents(IBimaruGrid grid, EventHandler<FieldValueChangedEventArgs<BimaruValue>> eventHandler)
        {
            foreach (GridPoint p in grid.AllPoints().Where(p => grid[p] != BimaruValue.UNDETERMINED))
            {
                eventHandler(this, new FieldValueChangedEventArgs<BimaruValue>(p, BimaruValue.UNDETERMINED));
            }
        }

        private static void ApplyChanges(IGame game, FieldsToChange<BimaruValue> changes)
        {
            int numChangedFields = 0;

            if (changes != null)
            {
                foreach (var c in changes)
                {
                    numChangedFields += (c.NewValue == game.Grid[c.Point]) ? 0 : 1;
                    game.Grid[c.Point] = c.NewValue;
                }
            }

            if (numChangedFields == 0)
            {
                throw new InvalidOperationException("Invalid Bimaru field changes (could lead to an infinite recursion).");
            }
        }

        private void HandleChangedEvents(IGame game, Queue<FieldValueChangedEventArgs<BimaruValue>> unhandledChangedEvents)
        {
            while (unhandledChangedEvents.Count > 0)
            {
                var e = unhandledChangedEvents.Dequeue();
                foreach (IFieldValueChangedRule rule in FieldChangedRules)
                {
                    rule.FieldValueChanged(game, e);
                }
            }
        }

        private void RunFullGridRules(IGame game, bool isFirstCall)
        {
            foreach (ISolverRule rule in FullGridRules.Where(rule => isFirstCall || !rule.ShallBeAppliedOnce))
            {
                rule.Solve(game);
            }
        }

        private int RunTrialAndErrorRule(IGame game)
        {
            if (game.IsSolved)
            {
                GridBackup.CloneToClipboard(game.Grid);
                return 1;
            }

            if (TrialRule == null)
            {
                return 0;
            }

            int numSolutions = 0;

            foreach (FieldsToChange<BimaruValue> changes in TrialRule.GetCompleteChangeTrials(game))
            {
                numSolutions += RunRulesInSavepoint(game, false, changes);

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
