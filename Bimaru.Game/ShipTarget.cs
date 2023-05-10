using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bimaru.Interface.Game;
using Bimaru.Interface.Utility;

namespace Bimaru.Game
{
    public class ShipTarget : IShipTarget
    {
        public ShipTarget()
        {
            targetNumberOfShipsPerLength = new SortedDictionary<int, int>();
        }

        public ShipTarget(int[] targetNumberOfShipsPerLength)
            : this()
        {
            foreach (var it in targetNumberOfShipsPerLength.Select((t, l) => new { TargetNumber = t, ShipLength = l + 1 }))
            {
                this[it.ShipLength] = it.TargetNumber;
            }
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
            return satisfiability switch
            {
                Satisfiability.VIOLATED => throw new InvalidBimaruGameException("Ship target is violated."),
                Satisfiability.SATISFIED => null,
                Satisfiability.SATISFIABLE => targetNumberOfShipsPerLength
                    .Last(p => p.Value > numberOfShipsPerLength[p.Key])
                    .Key,
                _ => targetNumberOfShipsPerLength.Last(p => p.Value > numberOfShipsPerLength[p.Key]).Key
            };
        }

        public IEnumerator<int> GetEnumerator()
        {
            return GetTargetNumberOfShipsEnumeratedByLength().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<int> GetTargetNumberOfShipsEnumeratedByLength()
        {
            var maxLength = targetNumberOfShipsPerLength.Keys.Max(i => (int?)i) ?? 0;

            return Enumerable.Range(1, maxLength).Select(shipLength => this[shipLength]);
        }
    }
}
