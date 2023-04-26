using Bimaru.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Bimaru.SolverUtil
{
    /// <summary>
    /// Try all possible locations for the longest, still missing ship.
    /// </summary>
    public class LongestMissingShip : ITrialAndErrorRule
    {
        /// <summary>
        /// To see why this rule does not produce disjoint trials, in general,
        /// suppose that two battleships are missing in a game with a unique
        /// solution. Then this rule will produce at least two trials for
        /// battleships, one for each correct location of a battleship.
        /// However, each of those two trials will lead to a solution
        /// (which is the same in both cases).
        /// </summary>
        public bool AreTrialsDisjoint => false;

        /// <summary>
        /// The set of trials contains all possible locations for the longest
        /// missing ship. If there is no missing ship, it produces a trial to
        /// set all UNDETERMINED fields to water. Hence, it is complete.
        /// </summary>
        public bool AreTrialsComplete => true;

        /// <inheritdoc/>
        public IEnumerable<FieldsToChange<BimaruValue>> GetChangeTrials(IGame game)
        {
            int? shipLength;

            try
            {
                shipLength = game.LengthOfLongestMissingShip;
            }
            catch (ArgumentOutOfRangeException e)
            {
                throw new InvalidBimaruGameException("", e);
            }

            if (shipLength.HasValue)
            {
                return GetCompatibleButNotEqualShipTrials(game, shipLength.Value);
            }
            else
            {
                // All ships are set => Only water missing
                return GetUndeterminedToWaterTrial(game);
            }
        }

        private IEnumerable<FieldsToChange<BimaruValue>> GetCompatibleButNotEqualShipTrials(IGame game, int shipLength)
        {
            foreach (var ship in GetVerticalShipLocations(game, shipLength).Where(s => s.IsCompatibleButNotEqualIn(game)))
            {
                yield return ship.Changes;
            }

            if (shipLength == 1)
            {
                // Single ships are already considered vertically
                yield break;
            }

            foreach (var ship in GetHorizontalShipLocations(game, shipLength).Where(s => s.IsCompatibleButNotEqualIn(game)))
            {
                yield return ship.Changes;
            }
        }

        private IEnumerable<ShipLocation> GetVerticalShipLocations(IGame game, int shipLength)
        {
            var rowIndexes = Enumerable.Range(0, game.Grid.NumberOfRows - shipLength + 1);

            var columnIndexes = Enumerable.Range(0, game.Grid.NumberOfColumns).
                Where(i => game.TargetNumberOfShipFieldsPerColumn[i] >= shipLength);

            foreach (var p in GetGridPoints(rowIndexes, columnIndexes))
            {
                yield return new ShipLocation(p, Direction.UP, shipLength);
            }
        }

        private static IEnumerable<GridPoint> GetGridPoints(IEnumerable<int> rowIndexes, IEnumerable<int> columnIndexes)
        {
            return from rowIndex in rowIndexes
                from columnIndex in columnIndexes
                select new GridPoint(rowIndex, columnIndex);
        }

        private static IEnumerable<ShipLocation> GetHorizontalShipLocations(IGame game, int shipLength)
        {
            var rowIndexes = Enumerable.Range(0, game.Grid.NumberOfRows).
                Where(i => game.TargetNumberOfShipFieldsPerRow[i] >= shipLength);

            var columnIndexes = Enumerable.Range(0, game.Grid.NumberOfColumns - shipLength + 1);

            foreach (var p in GetGridPoints(rowIndexes, columnIndexes))
            {
                yield return new ShipLocation(p, Direction.RIGHT, shipLength);
            }
        }

        private static IEnumerable<FieldsToChange<BimaruValue>> GetUndeterminedToWaterTrial(IGame game)
        {
            var changes = new FieldsToChange<BimaruValue>();

            foreach (var p in game.Grid.AllPoints().Where(p => !game.Grid[p].IsFullyDetermined()))
            {
                changes.Add(p, BimaruValue.WATER);
            }

            if (changes.Any())
            {
                yield return changes;
            }
        }
    }
}
