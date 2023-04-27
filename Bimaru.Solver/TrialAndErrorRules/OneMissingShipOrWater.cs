using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bimaru.Interface.Game;
using Bimaru.Interface.Solver;
using Bimaru.Interface.Utility;

namespace Bimaru.Solver.TrialAndErrorRules
{
    /// <summary>
    /// Finds a row or column where either only one
    /// WATER or only one ship is missing. Then it
    /// tries to use this single WATER or ship in
    /// all UNDETERMINED positions of the row or column.
    /// 
    /// If no such row or column exists, then the request
    /// is delegated to a fall-back rule.
    /// </summary>
    public class OneMissingShipOrWater : ITrialAndErrorRule
    {
        public OneMissingShipOrWater(ITrialAndErrorRule fallBackRule)
        {
            FallBackRule = fallBackRule;
        }

        private ITrialAndErrorRule FallBackRule
        {
            get;
        }

        /// <summary>
        /// This trial and error rule itself is disjoint
        /// because different trials contradict each other.
        /// 
        /// In the cases when the fall-back rule is used,
        /// then this rule inherits the characteristics
        /// from the fall-back rule. If then no fall-back
        /// rule exists, then no trials are produced, and
        /// hence they are disjoint.
        /// </summary>
        public bool AreTrialsDisjoint =>
            FallBackRule is null || FallBackRule.AreTrialsDisjoint;

        /// <summary>
        /// This trial and error rule itself is complete
        /// because if there is exactly one WATER or SHIP
        /// missing in a row or column, then one of the
        /// UNDETERMINED fields has to be it.
        /// 
        /// In the cases when the fall-back rule is used,
        /// then this rule inherits the characteristics
        /// from the fall-back rule. If then no fall-back
        /// rule exists, then no trials are produced, and
        /// hence it is not complete.
        /// </summary>
        public bool AreTrialsComplete =>
            !(FallBackRule is null) && FallBackRule.AreTrialsComplete;

        public IEnumerable<FieldsToChange<BimaruValue>> GetChangeTrials(IBimaruGame game)
        {
            var mostPromisingPoints = GetMostPromisingPoints(game);
            if (mostPromisingPoints != null)
            {
                return GenerateChangeTrials(mostPromisingPoints);
            }

            return FallBackRule?.GetChangeTrials(game) ?? Enumerable.Empty<FieldsToChange<BimaruValue>>();
        }

        private static OneMissingPoints GetMostPromisingPoints(IBimaruGame game)
        {
            return new List<OneMissingPoints>()
                {
                    GetOneMissingShipRow(game).Max(),
                    GetOneMissingWaterRow(game).Max(),
                    GetOneMissingShipColumn(game).Max(),
                    GetOneMissingWaterColumn(game).Max()
                }.Max();
        }

        private static IEnumerable<OneMissingPoints> GetOneMissingShipRow(IBimaruGame game)
        {
            return Enumerable.Range(0, game.Grid.NumberOfRows)
                .Where(i => game.NumberOfMissingShipFieldsPerRow(i) == 1)
                .Select(rowIndex => OneMissingPoints.ConstructFromRow(game, rowIndex, BimaruValueConstraint.SHIP));
        }

        private static IEnumerable<OneMissingPoints> GetOneMissingWaterRow(IBimaruGame game)
        {
            return Enumerable.Range(0, game.Grid.NumberOfRows)
                .Where(i => game.NumberOfMissingShipFieldsPerRow(i) ==
                            (game.Grid.NumberOfUndeterminedFieldsPerRow[i] - 1))
                .Select(rowIndex => OneMissingPoints.ConstructFromRow(game, rowIndex, BimaruValueConstraint.WATER));
        }

        private static IEnumerable<OneMissingPoints> GetOneMissingShipColumn(IBimaruGame game)
        {
            return Enumerable.Range(0, game.Grid.NumberOfColumns)
                .Where(i => game.NumberOfMissingShipFieldsPerColumn(i) == 1)
                .Select(columnIndex => OneMissingPoints.ConstructFromColumn(game, columnIndex, BimaruValueConstraint.SHIP));
        }

        private static IEnumerable<OneMissingPoints> GetOneMissingWaterColumn(IBimaruGame game)
        {
            return Enumerable.Range(0, game.Grid.NumberOfColumns)
                .Where(i => game.NumberOfMissingShipFieldsPerColumn(i) ==
                            (game.Grid.NumberOfUndeterminedFieldsPerColumn[i] - 1))
                .Select(columnIndex => OneMissingPoints.ConstructFromColumn(game, columnIndex, BimaruValueConstraint.WATER));
        }

        private static IEnumerable<FieldsToChange<BimaruValue>> GenerateChangeTrials(OneMissingPoints points)
        {
            var valueToSet = points.ShipOrWater.GetRepresentativeValue();
            foreach (var p in points)
            {
                yield return new FieldsToChange<BimaruValue>(p, valueToSet);
            }
        }

        /// <summary>
        /// UNDETERMINED grid points where exactly one
        /// can be WATER or exactly one can be a ship.
        /// </summary>
        private sealed class OneMissingPoints : IEnumerable<GridPoint>, IComparable<OneMissingPoints>
        {
            private OneMissingPoints(IEnumerable<GridPoint> points, BimaruValueConstraint shipOrWater)
            {
                Points = points;
                ShipOrWater = shipOrWater;
            }

            public static OneMissingPoints ConstructFromRow(IBimaruGame game, int rowIndex, BimaruValueConstraint shipOrWater)
            {
                return ConstructFromPoints(game, game.Grid.PointsOfRow(rowIndex), shipOrWater);
            }

            public static OneMissingPoints ConstructFromColumn(IBimaruGame game, int columnIndex, BimaruValueConstraint shipOrWater)
            {
                return ConstructFromPoints(game, game.Grid.PointsOfColumn(columnIndex), shipOrWater);
            }

            private static OneMissingPoints ConstructFromPoints(IBimaruGame game, IEnumerable<GridPoint> points, BimaruValueConstraint shipOrWater)
            {
                OneMissingPoints result = null;

                var undeterminedPoints = points.Where(p => game.Grid[p] == BimaruValue.UNDETERMINED).ToList();
                if (undeterminedPoints.Any())
                {
                    result = new OneMissingPoints(
                        undeterminedPoints,
                        shipOrWater);
                }

                return result;
            }

            private IEnumerable<GridPoint> Points
            {
                get;
            }

            public BimaruValueConstraint ShipOrWater
            {
                get;
            }

            /// <summary>
            /// Most promising candidate in terms of leading to a fast solver.
            /// Empirical heuristic.
            /// </summary>
            public static bool operator >(OneMissingPoints left, OneMissingPoints right)
            {
                if (left is null || right is null)
                {
                    return !(left is null);
                }

                // Rules of thumb to support this order:
                //
                // 1. The more ships set, the faster.
                //    More ships are set if one WATER is missing.
                //
                // 2. The more fields set, the faster.
                // 
                return (left.ShipOrWater == BimaruValueConstraint.WATER &&
                        right.ShipOrWater == BimaruValueConstraint.SHIP) ||
                       (left.ShipOrWater == right.ShipOrWater &&
                        left.Points.Count() > right.Points.Count());
            }

            public static bool operator <(OneMissingPoints left, OneMissingPoints right)
            {
                return right > left;
            }

            public IEnumerator<GridPoint> GetEnumerator()
            {
                return Points.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return Points.GetEnumerator();
            }

            public int CompareTo(OneMissingPoints other)
            {
                return this > other ? 1 : -1;
            }
        }
    }
}
