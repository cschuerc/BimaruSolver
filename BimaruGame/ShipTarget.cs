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

        public int this[int shipLength]
        {
            get
            {
                targetNumberOfShipsPerLength.TryGetValue(shipLength, out var numShips);
                return numShips;
            }
            set
            {
                if (shipLength <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(shipLength), shipLength, "Ship lengths must be positive.");
                }

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Number of ships must be non-negative.");
                }

                SetShipTarget(shipLength, value);
            }
        }

        private void SetShipTarget(int shipLength, int newNumberOfShips)
        {
            targetNumberOfShipsPerLength.TryGetValue(shipLength, out var oldNumberOfShips);
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

        public int? LongestShipLength
        {
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
            var isSatisfied = true;
            for (var shipLength = 0; shipLength < numberOfShipsPerLength.Count; shipLength++)
            {
                var numberOfShipsGap = this[shipLength] - numberOfShipsPerLength[shipLength];
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
            switch (satisfiability)
            {
                case Satisfiability.VIOLATED:
                    throw new ArgumentOutOfRangeException(nameof(numberOfShipsPerLength), numberOfShipsPerLength, "Ship target is violated.");
                case Satisfiability.SATISFIED:
                    return null;
                case Satisfiability.SATISFIABLE:
                default:
                    return targetNumberOfShipsPerLength.Last(p => p.Value > numberOfShipsPerLength[p.Key]).Key;
            }
        }
    }
}
