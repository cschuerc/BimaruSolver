using System.Linq;
using System.Collections.Generic;
using BimaruInterfaces;
using Utility;
using System;

namespace BimaruSolver
{
    /// <summary>
    /// Finds a row or column where either only one
    /// WATER or only one ship is missing. Then it
    /// tries to use this single WATER or ship in
    /// all UNDETERMINED positions of the row or column.
    /// 
    /// If no such row or column exists, then the request
    /// is delegated to a fall-back rule that was set in
    /// the constructor.
    /// 
    /// This trial and error rule itself is complete
    /// and disjoint. In the cases when the fall-back
    /// rule is used, then this rule inherits the
    /// characteristics from the fall-back rule.
    /// 
    /// Hence, this rule can be used to count the number
    /// of solutions if the fall-back rule is complete
    /// and disjoint as well.
    /// </summary>
    public class OneMissingShipOrWater : ITrialAndErrorRule
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fallBackRule"> Fall-back rule </param>
        public OneMissingShipOrWater(ITrialAndErrorRule fallBackRule)
        {
            FallBackRule = fallBackRule;
        }

        /// <summary>
        /// Fall-back rule
        /// </summary>
        protected ITrialAndErrorRule FallBackRule { get; private set; }

        /// <inheritdoc/>
        public bool AreTrialsDisjoint =>
            FallBackRule is null || FallBackRule.AreTrialsDisjoint;

        /// <summary>
        /// Grid points whose values are UNDETERMINED and
        /// where only one is WATER or only one is a ship.
        /// </summary>
        protected class OneMissing
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="undetermiedPoints"> Grid points whose values are UNDETERMINED. </param>
            /// <param name="missingType"> Type of value of which there is only one missing. </param>
            public OneMissing(IEnumerable<GridPoint> undetermiedPoints, BimaruValueConstraint missingType)
            {
                UndeterminedPoints = undetermiedPoints;
                MissingType = missingType;
            }

            /// <summary>
            /// UNDETERMINED grid points where only one is WATER or only one is a ship.
            /// </summary>
            public IEnumerable<GridPoint> UndeterminedPoints { get; private set; }

            /// <summary>
            /// Type of value of which there is only one missing.
            /// </summary>
            public BimaruValueConstraint MissingType { get; private set; }

            /// <summary>
            /// Comparison that empirically led to the fastest solver.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static bool operator>(OneMissing left, OneMissing right)
            {
                if (left is null || right is null)
                {
                    return !(left is null);
                }

                // Rules of thumb to support this order:
                //
                // 1. The more ships set, the faster.
                //    More ships set if one WATER is missing.
                //
                // 2. The more fields set, the faster.
                // 
                return (left.MissingType == BimaruValueConstraint.WATER &&
                        right.MissingType == BimaruValueConstraint.SHIP) ||
                       (left.MissingType == right.MissingType &&
                        left.UndeterminedPoints.Count() > right.UndeterminedPoints.Count());
            }

            /// <summary>
            /// See >.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static bool operator<(OneMissing left, OneMissing right)
            {
                return right > left;
            }
        }

        private OneMissing GetOneMissingRow(IGame game)
        {
            OneMissing max = null;

            foreach (int rowIndex in Enumerable.Range(0, game.Grid.NumRows))
            {
                var undeterminedPointsInRow = game.Grid.PointsOfRow(rowIndex).
                    Where(p => game.Grid.GetFieldValue(p) == BimaruValue.UNDETERMINED);

                if (game.MissingShipFieldsRow(rowIndex) == 1)
                {
                    var current = new OneMissing(undeterminedPointsInRow, BimaruValueConstraint.SHIP);
                    max = current > max ? current : max;
                }

                if (game.MissingShipFieldsRow(rowIndex) ==
                    (game.Grid.GetNumUndeterminedFieldsRow[rowIndex] - 1))
                {
                    var current = new OneMissing(undeterminedPointsInRow, BimaruValueConstraint.WATER);
                    max = current > max ? current : max;
                }
            }

            return max;
        }

        private OneMissing GetOneMissingColumn(IGame game)
        {
            OneMissing max = null;

            foreach (int columnIndex in Enumerable.Range(0, game.Grid.NumColumns))
            {
                var undeterminedPointsInColumn = game.Grid.PointsOfColumn(columnIndex).
                    Where(p => game.Grid.GetFieldValue(p) == BimaruValue.UNDETERMINED);

                if (game.MissingShipFieldsColumn(columnIndex) == 1)
                {
                    var current = new OneMissing(undeterminedPointsInColumn, BimaruValueConstraint.SHIP);
                    max = current > max ? current : max;
                }

                if (game.MissingShipFieldsColumn(columnIndex) ==
                    (game.Grid.GetNumUndeterminedFieldsColumn[columnIndex] - 1))
                {
                    var current = new OneMissing(undeterminedPointsInColumn, BimaruValueConstraint.WATER);
                    max = current > max ? current : max;
                }
            }

            return max;
        }

        private OneMissing GetOneMissing(IGame game)
        {
            OneMissing oneMissingRow = GetOneMissingRow(game);
            OneMissing oneMissingColumn = GetOneMissingColumn(game);

            return oneMissingRow > oneMissingColumn ? oneMissingRow : oneMissingColumn;
        }

        private IEnumerable<FieldsToChange<BimaruValue>> GenerateOneMissingChanges(OneMissing oneMissing)
        {
            var valueToSet = oneMissing.MissingType.GetRepresentativeValue();
            foreach (var p in oneMissing.UndeterminedPoints)
            {
                yield return new FieldsToChange<BimaruValue>(p, valueToSet);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FieldsToChange<BimaruValue>> GetCompleteChangeTrials(IGame game)
        {
            OneMissing oneMissing = GetOneMissing(game);
            if (oneMissing != null)
            {
                return GenerateOneMissingChanges(oneMissing);
            }

            if (FallBackRule == null)
            {
                throw new InvalidOperationException();
            }

            return FallBackRule.GetCompleteChangeTrials(game);
        }
    }
}
