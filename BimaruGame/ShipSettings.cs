using BimaruInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BimaruGame
{
    /// <summary>
    /// Standard implementation of IShipSettings
    /// </summary>
    [Serializable]
    public class ShipSettings : IShipSettings
    {
        private SortedDictionary<int, int> _numShipsPerLength;

        /// <summary>
        /// Instantiate ship settings with no ships
        /// </summary>
        public ShipSettings()
        {
            _numShipsPerLength = new SortedDictionary<int, int>();
        }

        /// <inheritdoc/>
        public int this[int length] {
            get
            {
                _numShipsPerLength.TryGetValue(length, out int numShips);
                return numShips;
            }
            set
            {
                if (length <= 0)
                {
                    throw new ArgumentOutOfRangeException("Ship lengths must be positive.");
                }

                _numShipsPerLength.TryGetValue(length, out int origNumShips);
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Number of ships must be non-negative.");
                }
                else if (value == 0)
                {
                    _numShipsPerLength.Remove(length);
                }
                else
                {
                    _numShipsPerLength[length] = value;
                }

                NumShipFields += (value - origNumShips) * length;
            }
        }

        /// <inheritdoc/>
        public int LongestShipLength {
            get
            {
                int longestLength = 0;

                if (_numShipsPerLength.Count > 0)
                {
                    longestLength = _numShipsPerLength.Last().Key;
                }

                return longestLength;
            }
        }

        /// <inheritdoc/>
        public int NumShipFields { get; private set; }
    }
}
