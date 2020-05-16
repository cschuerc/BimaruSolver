using Bimaru.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Bimaru.GameUtil
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


        private int _length;

        public int Length
        {
            get
            {
                return _length;
            }

            private set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("Tally length has to be > 0.");
                }

                _length = value;
            }
        }


        private readonly int[] targetNumberPerRowOrColumn;

        public int this[int index]
        {
            get
            {
                return targetNumberPerRowOrColumn[index];
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Negative tally entries are not allowed.");
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
            IReadOnlyList<int> numberOfFields,
            IReadOnlyList<int> additionalNumberOfFields)
        {
            if (Length != numberOfFields.Count ||
                Length != additionalNumberOfFields.Count)
            {
                throw new ArgumentOutOfRangeException("Grid tally satisfiability has to be derived from the same number of rows/columns.");
            }

            bool isSatisfied = true;
            for (int index = 0; index < Length; index++)
            {
                int numberOfMissingFields = this[index] - numberOfFields[index];
                if (numberOfMissingFields < 0 ||
                    numberOfMissingFields > additionalNumberOfFields[index])
                {
                    return Satisfiability.VIOLATED;
                }
                else if (numberOfMissingFields != 0)
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
