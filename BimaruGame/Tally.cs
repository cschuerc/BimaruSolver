using BimaruInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace BimaruGame
{
    /// <summary>
    /// Standard implementation of ITally
    /// </summary>
    [Serializable]
    public class Tally : ITally
    {
        /// <summary>
        /// Instantiates a tally of given length
        /// </summary>
        /// <param name="length"> Length of the tally </param>
        public Tally(int length)
        {
            Length = length;

            _numShipFields = new int[length].InitValues(0);

            Sum = _numShipFields.Sum();
        }


        private int[] _numShipFields;

        /// <inheritdoc/>
        public int this[int index]
        {
            get
            {
                return _numShipFields[index];
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (_numShipFields[index] != value)
                {
                    Sum += value - _numShipFields[index];

                    _numShipFields[index] = value;
                }
            }
        }


        private int _length;

        /// <inheritdoc/>
        public int Length
        {
            get
            {
                return _length;
            }

            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                _length = value;
            }
        }

        /// <inheritdoc/>
        public int Sum
        {
            get;
            private set;
        }

        /// <inheritdoc/>
        public IEnumerator<int> GetEnumerator()
        {
            return ((IEnumerable<int>)_numShipFields).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _numShipFields.GetEnumerator();
        }
    }
}
