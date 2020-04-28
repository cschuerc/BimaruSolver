using System;
using System.Linq;
using System.Collections.Generic;
using Utility;

namespace BimaruInterfaces
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
        public Solver(IEnumerable<IFieldChangedRule> fieldChangedRules,
            IEnumerable<IFullGridRule> fullGridRules,
            ITrialAndErrorRule trialRule)
        {
            FieldChangedRules = fieldChangedRules;
            FullGridRules = fullGridRules;
            TrialRule = trialRule;
        }

        /// <inheritdoc/>
        public virtual void Solve(IGame game)
        {
            if (!game.IsValid)
            {
                throw new InvalidBimaruGame();
            }

            InitUnhandledFieldChanges(game.Grid);

            void FieldChangedHandler(object sender, FieldValueChangedEventArgs<BimaruValue> e)
            {
                CheckIsChangeValid(game, e);
                UnhandledFieldChanges.Enqueue(e);
            }

            game.Grid.FieldValueChanged += FieldChangedHandler;

            game.Grid.SetSavePoint();

            try
            {
                RunRules(game, true);
            }
            catch (InvalidBimaruGame)
            {
            }
            finally
            {
                if (!game.IsSolved)
                {
                    game.Grid.Rollback();
                }

                game.Grid.FieldValueChanged -= FieldChangedHandler;
            }
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
        #endregion

        #region Handle field changes
        private Queue<FieldValueChangedEventArgs<BimaruValue>> UnhandledFieldChanges
        {
            get;
            set;
        }

        private void InitUnhandledFieldChanges(IGrid grid)
        {
            UnhandledFieldChanges = new Queue<FieldValueChangedEventArgs<BimaruValue>>();

            foreach (GridPoint p in grid.AllPoints().Where(p => grid.GetFieldValue(p) != BimaruValue.UNDETERMINED))
            {
                UnhandledFieldChanges.Enqueue(new FieldValueChangedEventArgs<BimaruValue>(p, BimaruValue.UNDETERMINED));
            }
        }

        private void HandleFieldChanges(IGame game)
        {
            while (UnhandledFieldChanges.Count > 0)
            {
                var e = UnhandledFieldChanges.Dequeue();
                foreach (IFieldChangedRule rule in FieldChangedRules)
                {
                    rule.FieldValueChanged(game, e);
                }
            }
        }

        private void CheckIsChangeValid(IGame game, FieldValueChangedEventArgs<BimaruValue> e)
        {
            BimaruValue newValue = game.Grid.GetFieldValue(e.Point);
            if (!game.IsValid || !e.OriginalValue.IsCompatibleChange(newValue))
            {
                throw new InvalidFieldChange();
            }
        }
        #endregion

        #region Run rules
        private void RunRules(IGame game, bool isFirstCall)
        {
            HandleFieldChanges(game);
            RunFullGridRules(game, isFirstCall);
            HandleFieldChanges(game);

            RunTrialAndErrorRule(game);
        }

        private void RunFullGridRules(IGame game, bool isFirstCall)
        {
            foreach (IFullGridRule rule in FullGridRules.Where(rule => isFirstCall || !rule.ShallBeAppliedOnce))
            {
                rule.Solve(game);
            }
        }

        private void RunTrialAndErrorRule(IGame game)
        {
            if (game.Grid.IsFullyDetermined || TrialRule == null)
            {
                return;
            }

            foreach (FieldsToChange<BimaruValue> changes in TrialRule.GetCompleteChangeTrials(game))
            {
                game.Grid.SetSavePoint();

                try
                {
                    ApplyChanges(game, changes);
                    RunRules(game, false);
                }
                catch (InvalidBimaruGame)
                {
                }
                finally
                {
                    if (game.IsSolved)
                    {
                        game.Grid.RemovePrevious();
                    }
                    else
                    {
                        game.Grid.Rollback();
                    }
                }

                if (game.IsSolved)
                {
                    return;
                }
            }
        }

        private void ApplyChanges(IGame game, FieldsToChange<BimaruValue> changes)
        {
            int numChangedFields = 0;
            foreach (var c in changes)
            {
                numChangedFields += (c.NewValue == game.Grid.GetFieldValue(c.Point)) ? 0 : 1;
                game.Grid.SetFieldValue(c.Point, c.NewValue);
            }

            if (numChangedFields == 0)
            {
                throw new InvalidOperationException("Invalid Bimaru field changes (could lead to an infinite recursion).");
            }
        }
        #endregion
    }
}
