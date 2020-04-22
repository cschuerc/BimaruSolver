using BimaruInterfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using Utility;

namespace BimaruSolver
{
    /// <summary>
    /// Generic Bimaru solver
    /// </summary>
    public class Solver : ISolver
    {
        /// <summary>
        /// Creates a generic Bimaru solver.
        /// </summary>
        /// <param name="fieldChangedRules"> Solving rules triggered after every field change. </param>
        /// <param name="fullGridRules"> General solving rules without guessing. </param>
        /// <param name="trialRule"> Single rule that tries out different possibilities. </param>
        public Solver(IList<IFieldChangedRule> fieldChangedRules,
            IList<IFullGridRule> fullGridRules,
            ITrialAndErrorRule trialRule)
        {
            FieldChangedRules = (fieldChangedRules == null) ?
                new List<IFieldChangedRule>() :
                fieldChangedRules.ToList();

            FullGridRules = (fullGridRules == null) ?
                new List<IFullGridRule>() :
                fullGridRules.ToList();

            TrialRule = trialRule;
        }

        /// <summary>
        /// Solving rules triggered after every field change.
        /// </summary>
        protected IList<IFieldChangedRule> FieldChangedRules
        {
            get;
            private set;
        }

        /// <summary>
        /// General solving rules without guessing.
        /// </summary>
        protected IList<IFullGridRule> FullGridRules
        {
            get;
            private set;
        }

        /// <summary>
        /// Single rule that tries out different possibilities
        /// </summary>
        protected ITrialAndErrorRule TrialRule
        {
            get;
            private set;
        }

        /// <inheritdoc/>
        public virtual void Solve(IGame game)
        {
            if (!game.IsValid)
            {
                throw new InvalidBimaruGame("This Bimaru game is invalid.");
            }

            void FieldChangedHandler(object sender, FieldValueChangedEventArgs<FieldValues> e)
            {
                CheckIsChangeValid(game, e);
                RunFieldValueChangedRules(game, e);
            }

            game.Grid.FieldValueChanged += FieldChangedHandler;

            game.Grid.SetSavePoint();

            try
            {
                RunInitialFieldValueChangedRules(game);

                RunFullGridRules(game);
                RunTrialAndErrorRule(game);
            }
            catch (InvalidFieldChange)
            {
                // Invalid field changes are caused either by a wrong rule
                // or by an unsolvable game. Since we cannot distinguish the
                // two situations and because the rules should be well tested
                // we assume an unsolvable game => Just do nothing.
            }
            finally
            {
                game.Grid.FieldValueChanged -= FieldChangedHandler;

                if (!game.IsSolved)
                {
                    game.Grid.Rollback();
                }
            }
        }

        private void RunFieldValueChangedRules(IGame game, FieldValueChangedEventArgs<FieldValues> e)
        {
            foreach (IFieldChangedRule rule in FieldChangedRules)
            {
                rule.FieldValueChanged(game, e);
            }
        }

        private void CheckIsChangeValid(IGame game, FieldValueChangedEventArgs<FieldValues> e)
        {
            FieldValues newValue = game.Grid.GetFieldValue(e.Point);
            if (!game.IsValid || !e.OriginalValue.IsCompatibleChange(newValue))
            {
                throw new InvalidFieldChange();
            }
        }

        private void RunInitialFieldValueChangedRules(IGame game)
        {
            for (int rowIndex = 0; rowIndex < game.Grid.NumRows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < game.Grid.NumColumns; columnIndex++)
                {
                    IGridPoint point = new GridPoint(rowIndex, columnIndex);
                    if (game.Grid.GetFieldValue(point) != FieldValues.UNDETERMINED)
                    {
                        var e = new FieldValueChangedEventArgs<FieldValues>(point, FieldValues.UNDETERMINED);
                        RunFieldValueChangedRules(game, e);
                    }
                }
            }
        }

        private void RunFullGridRules(IGame game)
        {
            foreach (IFullGridRule rule in FullGridRules)
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

            foreach (IEnumerable<FieldToChange> changes in TrialRule.GetCompleteChangeTrials(game))
            {
                game.Grid.SetSavePoint();

                try
                {
                    ApplyChangesAndRunRules(game, changes);
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

        private void ApplyChangesAndRunRules(IGame game, IEnumerable<FieldToChange> changes)
        {
            try
            {
                int numChangedFields = 0;
                foreach (FieldToChange c in changes)
                {
                    numChangedFields += (c.NewValue == game.Grid.GetFieldValue(c.Point)) ? 0 : 1;
                    game.Grid.SetFieldValue(c.Point, c.NewValue);
                }

                if (numChangedFields == 0)
                {
                    // Prevent an infinite recursion from changes that don't change anything.
                    throw new InvalidOperationException();
                }

                RunFullGridRules(game);
            }
            catch (InvalidFieldChange)
            {

            }

            RunTrialAndErrorRule(game);
        }
    }
}
