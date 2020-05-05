using System.Collections.Generic;
using System.Linq;
using BimaruInterfaces;
using Utility;

namespace BimaruSolver
{
    /// <summary>
    /// Try all possible locations for the longest, still missing ship.
    /// 
    /// Note that this rule does not produce disjoint trials and hence
    /// it cannot be used to count the number of solutions in a Bimaru
    /// game. To see this, suppose that two battleships are missing in
    /// a game with a unique solution. Then this rule will produce at
    /// least two trials for battleships, one for each correct location
    /// of a battleship. However, each of those two trials will lead to
    /// a solution (which is the same in both cases). A solver that just
    /// counts the number of trial paths with solutions will count both
    /// paths separately.
    /// </summary>
    public class LongestMissingShip : ITrialAndErrorRule
    {
        /// <inheritdoc/>
        public bool AreTrialsDisjoint => false;

        private int LengthOfLongestMissingShip(IGame game)
        {
            int length = game.ShipSettings.LongestShipLength;
            while (length > 0)
            {
                int numShipsGap = game.ShipSettings[length] - game.Grid.GetNumShips[length];
                if (numShipsGap < 0)
                {
                    throw new InvalidBimaruGame();
                }
                else if (numShipsGap > 0)
                {
                    break;
                }

                length--;
            }

            return length;
        }

        private bool IsCompatibleButNotEqual(IGame game, FieldsToChange<BimaruValue> changes)
        {
            bool isEqual = true;

            foreach (var c in changes)
            {
                BimaruValue currentValue = game.Grid[c.Point];

                if (!currentValue.IsCompatibleChange(c.NewValue))
                {
                    return false;
                }

                isEqual = isEqual && (currentValue == c.NewValue);
            }

            return !isEqual;
        }

        private FieldsToChange<BimaruValue> UndeterminedToWaterChanges(IGame game)
        {
            var changes = new FieldsToChange<BimaruValue>();

            foreach (GridPoint p in game.Grid.AllPoints().Where(p => !game.Grid[p].IsFullyDetermined()))
            {
                changes.Add(p, BimaruValue.WATER);
            }

            return changes.Count() > 0 ? changes: null;
        }

        private IEnumerable<FieldsToChange<BimaruValue>> GetTrialsForVerticalShips(IGame game, int shipLength)
        {
            int numStartRows = game.Grid.NumRows - shipLength + 1;
            foreach (int columnIndex in Enumerable.Range(0, game.Grid.NumColumns).Where(i => game.ColumnTally[i] >= shipLength))
            {
                foreach (int rowIndex in Enumerable.Range(0, numStartRows))
                {
                    GridPoint p = new GridPoint(rowIndex, columnIndex);
                    var shipFields = BimaruValueExtensions.FieldValuesOfShip(Direction.UP, shipLength);
                    var changes = new FieldsToChange<BimaruValue>(p, Direction.UP, shipFields);

                    if (IsCompatibleButNotEqual(game, changes))
                    {
                        yield return changes;
                    }
                }
            }
        }

        private IEnumerable<FieldsToChange<BimaruValue>> GetTrialsForHorizontalShips(IGame game, int shipLength)
        {
            int numStartColumns = game.Grid.NumColumns - shipLength + 1;
            foreach (int rowIndex in Enumerable.Range(0, game.Grid.NumRows).Where(i => game.RowTally[i] >= shipLength))
            {
                foreach (int columnIndex in Enumerable.Range(0, numStartColumns))
                {
                    GridPoint p = new GridPoint(rowIndex, columnIndex);
                    var shipFields = BimaruValueExtensions.FieldValuesOfShip(Direction.RIGHT, shipLength);
                    var changes = new FieldsToChange<BimaruValue>(p, Direction.RIGHT, shipFields);

                    if (IsCompatibleButNotEqual(game, changes))
                    {
                        yield return changes;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<FieldsToChange<BimaruValue>> GetCompleteChangeTrials(IGame game)
        {
            int shipLength = LengthOfLongestMissingShip(game);
            
            if (shipLength <= 0)
            {
                // All ships are set => Set the non-determined fields to water
                var trial = UndeterminedToWaterChanges(game);

                if (trial != null)
                {
                    yield return trial;
                }

                yield break;
            }

            foreach (var trial in GetTrialsForVerticalShips(game, shipLength))
            {
                yield return trial;
            }

            if (shipLength == 1)
            {
                // Single ships are already considered
                yield break;
            }

            foreach (var trial in GetTrialsForHorizontalShips(game, shipLength))
            {
                yield return trial;
            }
        }
    }
}
