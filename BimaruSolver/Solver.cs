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
        /// <param name="shallCountSolutions"> True, if the solver shall count the number of solutions. </param>
        public Solver(IEnumerable<IFieldChangedRule> fieldChangedRules,
            IEnumerable<IFullGridRule> fullGridRules,
            ITrialAndErrorRule trialRule,
            bool shallCountSolutions = false)
        {
            FieldChangedRules = fieldChangedRules;
            FullGridRules = fullGridRules;
            TrialRule = trialRule;
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
                game.Grid.RestoreFromClipboard();
            }

            return numSolutions;
        }

        #region Rules
        private List<IFieldChangedRule> _fieldChangedRules;

        /// <summary>
        /// Solving rules triggered after every field change.
        /// </summary>
        protected IEnumerable<IFieldChangedRule> FieldChangedRules
        {
            get
            {
                return _fieldChangedRules;
            }

            private set
            {
                _fieldChangedRules = new List<IFieldChangedRule>();

                if (value != null)
                {
                    _fieldChangedRules.AddRange(value);
                }
            }
        }

        private List<IFullGridRule> _fullGridRules;

        /// <summary>
        /// General solving rules without guessing.
        /// </summary>
        protected IEnumerable<IFullGridRule> FullGridRules
        {
            get
            {
                return _fullGridRules;
            }

            private set
            {
                _fullGridRules = new List<IFullGridRule>();

                if (value != null)
                {
                    _fullGridRules.AddRange(value);
                }
            }
        }

        /// <summary>
        /// Single rule that tries out different possibilities
        /// </summary>
        protected ITrialAndErrorRule TrialRule
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether the solver shall stop and return the first discovered solution.
        /// </summary>
        protected bool ShallCountSolutions
        {
            get;
            private set;
        }
        #endregion

        #region Run rules
        private int RunRulesInSavepoint(IGame game, bool isFirstCall, FieldsToChange<BimaruValue> changes = null)
        {
            int numSolutions = 0;

            game.Grid.SetSavePoint();

            try
            {
                numSolutions = RunRules(game, isFirstCall, changes);
            }
            catch (InvalidBimaruGame)
            {
            }
            finally
            {
                game.Grid.Rollback();
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
            if (!game.IsValid || !e.OriginalValue.IsCompatibleChange(newValue))
            {
                throw new InvalidFieldChange();
            }
        }

        private void FireInitialFieldChangedEvents(IGrid grid, EventHandler<FieldValueChangedEventArgs<BimaruValue>> eventHandler)
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
                foreach (IFieldChangedRule rule in FieldChangedRules)
                {
                    rule.FieldValueChanged(game, e);
                }
            }
        }

        private void RunFullGridRules(IGame game, bool isFirstCall)
        {
            foreach (IFullGridRule rule in FullGridRules.Where(rule => isFirstCall || !rule.ShallBeAppliedOnce))
            {
                rule.Solve(game);
            }
        }

        private int RunTrialAndErrorRule(IGame game)
        {
            if (game.IsSolved)
            {
                game.Grid.CloneToClipboard();
                return 1;
            }

            if (game.Grid.IsFullyDetermined || TrialRule == null)
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
