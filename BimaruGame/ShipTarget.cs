using Bimaru.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bimaru.GameUtil
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

        public int? LongestShipLength {
            get
            {
                int? longestLength = null;
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
                int numberOfShipsGap = this[shipLength] - numberOfShipsPerLength[shipLength];
                if (numberOfShipsGap < 0)
                {
                    return Satisfiability.VIOLATED;
                }
                else if (numberOfShipsGap != 0)
                {
                    isSatisfied = false;
                }
            }

            isSatisfied = isSatisfied && (LongestShipLength ?? 0) < numberOfShipsPerLength.Count;

            return isSatisfied ? Satisfiability.SATISFIED : Satisfiability.SATISFIABLE;
        }

        public int? LengthOfLongestMissingShip(IReadOnlyList<int> numberOfShipsPerLength)
        {
            var satisfiability = GetSatisfiability(numberOfShipsPerLength);
            if (satisfiability == Satisfiability.VIOLATED)
            {
                throw new ArgumentOutOfRangeException("Ship target is violated.");
            }
            else if (satisfiability == Satisfiability.SATISFIED)
            {
                return null;
            }

            return targetNumberOfShipsPerLength.Last(p => p.Value > numberOfShipsPerLength[p.Key]).Key;
        }
    }
}
