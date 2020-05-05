using BimaruInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BimaruGame
{
    /// <summary>
    /// Standard implementation of a Bimaru game
    /// </summary>
    [Serializable]
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
            ShipSettings = settings;
            Grid = grid;

            if (rowTally.Length != grid.NumRows || columnTally.Length != grid.NumColumns)
            {
                throw new ArgumentOutOfRangeException("The number of rows/columns does not agree between tallies and grid.");
            }
        }

        private enum Satisfiability
        {
            /// <summary>
            /// The constraint is violated
            /// </summary>
            VIOLATED,

            /// <summary>
            /// The constraint is not satisfied but also not violated
            /// </summary>
            SATISFIABLE,

            /// <summary>
            /// The constraint is satisfied with equality
            /// </summary>
            SATISFIED
        }

        #region Tallies
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

        private Satisfiability RowTallySatisfiability
        {
            get
            {
                bool isSatisfied = true;
                for (int rowIndex = 0; rowIndex < Grid.NumRows; rowIndex++)
                {
                    int numMissingShipFields = MissingShipFieldsRow(rowIndex);
                    if (numMissingShipFields < 0 || numMissingShipFields > Grid.GetNumUndeterminedFieldsRow[rowIndex])
                    {
                        return Satisfiability.VIOLATED;
                    }
                    else if (numMissingShipFields != 0)
                    {
                        isSatisfied = false;
                    }
                }

                return isSatisfied ? Satisfiability.SATISFIED : Satisfiability.SATISFIABLE;
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

        private Satisfiability ColumnTallySatisfiability
        {
            get
            {
                bool isSatisfied = true;
                for (int columnIndex = 0; columnIndex < Grid.NumColumns; columnIndex++)
                {
                    int numMissingShipFields = MissingShipFieldsColumn(columnIndex);
                    if (numMissingShipFields < 0 || numMissingShipFields > Grid.GetNumUndeterminedFieldsColumn[columnIndex])
                    {
                        return Satisfiability.VIOLATED;
                    }
                    else if (numMissingShipFields != 0)
                    {
                        isSatisfied = false;
                    }
                }

                return isSatisfied ? Satisfiability.SATISFIED : Satisfiability.SATISFIABLE;
            }
        }
        #endregion

        #region Ship settings
        private IShipSettings _shipSettings;

        /// <inheritdoc/>
        public IShipSettings ShipSettings
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

        private Satisfiability ShipSettingsSatisfiability
        {
            get
            {
                IReadOnlyList<int> numShipsPerLength = Grid.GetNumShips;

                bool areEqual = true;
                for (int length = 0; length < numShipsPerLength.Count; length++)
                {
                    int gap = ShipSettings[length] - numShipsPerLength[length];
                    if (gap < 0)
                    {
                        return Satisfiability.VIOLATED;
                    }
                    else if (gap != 0)
                    {
                        areEqual = false;
                    }
                }

                bool isSatisfied = areEqual && ShipSettings.LongestShipLength < numShipsPerLength.Count;

                return isSatisfied ? Satisfiability.SATISFIED : Satisfiability.SATISFIABLE;
            }
        }
        #endregion

        #region Grid
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
        #endregion

        #region Game properties
        /// <inheritdoc/>
        public bool IsUnsolvable
        {
            get
            {
                return RowTally.Any(t => t > Grid.NumColumns) ||
                    RowTally.Sum != ShipSettings.NumShipFields ||
                    ColumnTally.Any(t => t > Grid.NumRows) ||
                    ColumnTally.Sum != ShipSettings.NumShipFields ||
                    ShipSettings.LongestShipLength > Math.Max(Grid.NumColumns, Grid.NumRows);
            }
        }

        /// <inheritdoc/>
        public bool IsValid
        {
            get
            {
                return !IsUnsolvable &&
                    Grid.IsValid &&
                    RowTallySatisfiability != Satisfiability.VIOLATED &&
                    ColumnTallySatisfiability != Satisfiability.VIOLATED &&
                    ShipSettingsSatisfiability != Satisfiability.VIOLATED;
            }
        }

        /// <inheritdoc/>
        public bool IsSolved
        {
            get
            {
                return !IsUnsolvable &&
                    Grid.IsValid &&
                    Grid.IsFullyDetermined && 
                    RowTallySatisfiability == Satisfiability.SATISFIED &&
                    ColumnTallySatisfiability == Satisfiability.SATISFIED &&
                    ShipSettingsSatisfiability == Satisfiability.SATISFIED;
            }
        }
        #endregion
    }
}
