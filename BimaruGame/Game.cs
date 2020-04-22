using BimaruInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BimaruGame
{
    /// <summary>
    /// Implementatio of the Bimaru game
    /// </summary>
    public class Game : IGame
    {
        /// <summary>
        /// Instantiate new Bimaru game.
        /// </summary>
        /// <param name="rowTally"> Row tally </param>
        /// <param name="columnTally"> Column tally </param>
        /// <param name="settings"> Ship settings </param>
        /// <param name="grid"> Bimaru grid </param>
        public Game(ITally rowTally,
            ITally columnTally,
            IShipSettings settings,
            IRollbackGrid grid)
        {
            RowTally = rowTally;
            ColumnTally = columnTally;
            Settings = settings;
            Grid = grid;

            if (rowTally.Length != grid.NumRows ||
                columnTally.Length != grid.NumColumns)
            {
                throw new ArgumentOutOfRangeException("The number of rows/columns does not agree between tallies and grid.");
            }
        }


        private ITally _rowTally;

        /// <inheritdoc/>
        public ITally RowTally
        {
            get
            {
                return _rowTally;
            }

            private set
            {
                _rowTally = value ?? throw new ArgumentNullException("RowTally");
            }
        }


        private ITally _columnTally;

        /// <inheritdoc/>
        public ITally ColumnTally
        {
            get
            {
                return _columnTally;
            }

            private set
            {
                _columnTally = value ?? throw new ArgumentNullException("ColumnTally");
            }
        }


        private IShipSettings _shipSettings;

        /// <inheritdoc/>
        public IShipSettings Settings
        {
            get
            {
                return _shipSettings;
            }

            private set
            {
                _shipSettings = value ?? throw new ArgumentNullException("ShipSettings");
            }
        }


        private IRollbackGrid _grid;

        /// <inheritdoc/>
        public IRollbackGrid Grid
        {
            get
            {
                return _grid;
            }

            private set
            {
                _grid = value ?? throw new ArgumentNullException("Grid");
            }
        }

        /// <inheritdoc/>
        public int MissingShipFieldsRow(int index)
        {
            return _rowTally[index] - _grid.GetNumShipFieldsRow[index];
        }

        /// <inheritdoc/>
        public int MissingShipFieldsColumn(int index)
        {
            return _columnTally[index] - _grid.GetNumShipFieldsColumn[index];
        }

        private bool IsRowTallySatisfied
            => RowTally.SequenceEqual(Grid.GetNumShipFieldsRow);

        private bool IsColumnTallySatisfied
            => ColumnTally.SequenceEqual(Grid.GetNumShipFieldsColumn);

        private bool AreShipSettingsSatisfied
        {
            get
            {
                IReadOnlyList<int> numShipsPerLength = Grid.GetNumShips;

                int longestLength = Settings.LongestShipLength;

                if (longestLength > (numShipsPerLength.Count - 1))
                {
                    return false;
                }

                for (int length = 0; length < numShipsPerLength.Count; length++)
                {
                    if (numShipsPerLength[length] != Settings[length])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <inheritdoc/>
        public bool IsUnsolvable
        {
            get
            {
                return RowTally.Sum != Settings.NumShipFields ||
                    ColumnTally.Sum != Settings.NumShipFields ||
                    Settings.LongestShipLength > Math.Max(Grid.NumColumns, Grid.NumRows);
            }
        }

        private bool IsRowTallySatisfiable(int rowIndex)
        {
            return RowTally[rowIndex] >= Grid.GetNumShipFieldsRow[rowIndex] &&
                MissingShipFieldsRow(rowIndex) <= Grid.GetNumEmptyFieldsRow[rowIndex];
        }

        private bool IsColumnTallySatisfiable(int columnIndex)
        {
            return ColumnTally[columnIndex] >= Grid.GetNumShipFieldsColumn[columnIndex] &&
                MissingShipFieldsColumn(columnIndex) <= Grid.GetNumEmptyFieldsColumn[columnIndex];
        }

        /// <inheritdoc/>
        public bool IsValid
        {
            get
            {
                return !IsUnsolvable &&
                    Grid.IsValid &&
                    new int[Grid.NumRows].All(rowIndex => IsRowTallySatisfiable(rowIndex)) &&
                    new int[Grid.NumColumns].All(columnIndex => IsColumnTallySatisfiable(columnIndex));
            }
        }

        /// <inheritdoc/>
        public bool IsSolved
        {
            get
            {
                return IsValid &&
                    Grid.IsFullyDetermined && 
                    IsRowTallySatisfied &&
                    IsColumnTallySatisfied &&
                    AreShipSettingsSatisfied;
            }
        }
    }
}
