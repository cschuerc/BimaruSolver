using BimaruInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BimaruGame
{
    [Serializable]
    public class ShipTarget : IShipTarget
    {
        public ShipTarget()
        {
            targetNumberOfShipsPerLength = new SortedDictionary<int, int>();
        }


        private readonly SortedDictionary<int, int> targetNumberOfShipsPerLength;

        public int this[int shipLength] {
            get
            {
                targetNumberOfShipsPerLength.TryGetValue(shipLength, out int numShips);
                return numShips;
            }
            set
            {
                if (shipLength <= 0)
                {
                    throw new ArgumentOutOfRangeException("Ship lengths must be positive.");
                }

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Number of ships must be non-negative.");
                }

                SetShipTarget(shipLength, value);
            }
        }

        private void SetShipTarget(int shipLength, int newNumberOfShips)
        {
            targetNumberOfShipsPerLength.TryGetValue(shipLength, out int oldNumberOfShips);
            if (newNumberOfShips == 0)
            {
                targetNumberOfShipsPerLength.Remove(shipLength);
            }
            else
            {
                targetNumberOfShipsPerLength[shipLength] = newNumberOfShips;
            }

            TotalShipFields += (newNumberOfShips - oldNumberOfShips) * shipLength;
        }

        public int LongestShipLength {
            get
            {
                int longestLength = 0;
                if (targetNumberOfShipsPerLength.Count > 0)
                {
                    longestLength = targetNumberOfShipsPerLength.Last().Key;
                }

                return longestLength;
            }
        }

        public int TotalShipFields
        {
            get;
            private set;
        }

        public Satisfiability GetSatisfiability(IReadOnlyList<int> numberOfShipsPerLength)
        {
            bool isSatisfied = true;
            for (int shipLength = 0; shipLength < numberOfShipsPerLength.Count; shipLength++)
            {
                int gap = this[shipLength] - numberOfShipsPerLength[shipLength];
                if (gap < 0)
                {
                    return Satisfiability.VIOLATED;
                }
                else if (gap != 0)
                {
                    isSatisfied = false;
                }
            }

            isSatisfied = isSatisfied && LongestShipLength < numberOfShipsPerLength.Count;

            return isSatisfied ? Satisfiability.SATISFIED : Satisfiability.SATISFIABLE;
        }
    }
}
