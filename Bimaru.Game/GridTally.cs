using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bimaru.Interface;
using Bimaru.Interface.Game;
using Utility;

namespace Bimaru.Game
{
    [Serializable]
    public class GridTally : IGridTally
    {
        /// <param name="tallyLength"> Number of rows/columns </param>
        public GridTally(int tallyLength)
        {
            Length = tallyLength;

            targetNumberPerRowOrColumn = new int[tallyLength];
            targetNumberPerRowOrColumn.InitValues(0);

            Total = targetNumberPerRowOrColumn.Sum();
        }


        private int length;

        public int Length
        {
            get => length;

            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                length = value;
            }
        }


        private readonly int[] targetNumberPerRowOrColumn;

        public int this[int index]
        {
            get => targetNumberPerRowOrColumn[index];

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                Total += value - targetNumberPerRowOrColumn[index];
                targetNumberPerRowOrColumn[index] = value;
            }
        }

        public int Total
        {
            get;
            private set;
        }

        public Satisfiability GetSatisfiability(
            IReadOnlyList<int> numberOfShipFields,
            IReadOnlyList<int> numberOfUndeterminedFields)
        {
            if (Length != numberOfShipFields.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfShipFields), numberOfShipFields,
                    "Grid tally satisfiability has to be derived from the same number of rows/columns.");
            }

            if (Length != numberOfUndeterminedFields.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfUndeterminedFields), numberOfUndeterminedFields,
                    "Grid tally satisfiability has to be derived from the same number of rows/columns.");
            }

            var isSatisfied = true;
            for (var index = 0; index < Length; index++)
            {
                var numberOfMissingShipField = this[index] - numberOfShipFields[index];
                if (numberOfMissingShipField < 0 ||
                    numberOfMissingShipField > numberOfUndeterminedFields[index])
                {
                    return Satisfiability.VIOLATED;
                }

                if (numberOfMissingShipField != 0)
                {
                    isSatisfied = false;
                }
            }

            return isSatisfied ? Satisfiability.SATISFIED : Satisfiability.SATISFIABLE;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return ((IEnumerable<int>)targetNumberPerRowOrColumn).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return targetNumberPerRowOrColumn.GetEnumerator();
        }
    }
}
