using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Bimaru.Interface.Game;
using Bimaru.Interface.Utility;
using Bimaru.Utility;
using Newtonsoft.Json;

namespace Bimaru.Game
{
    [JsonObject]
    public class GridTally : IGridTally
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length">Number of rows/columns</param>
        [JsonConstructor]
        public GridTally(int length)
        {
            Length = length;

            targetNumberPerIndex = new int[length];
            targetNumberPerIndex.InitValues(0);

            Total = targetNumberPerIndex.Sum();
        }

        public GridTally(int[] targetNumberPerIndex)
            : this(targetNumberPerIndex.Length)
        {
            foreach (var it in targetNumberPerIndex.Select((t, i) => new { TargetNumber = t, Index = i }))
            {
                this[it.Index] = it.TargetNumber;
            }
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

        [JsonProperty]
        private readonly int[] targetNumberPerIndex;

        [JsonIgnore]
        public int this[int index]
        {
            get => targetNumberPerIndex[index];

            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                Total += value - targetNumberPerIndex[index];
                targetNumberPerIndex[index] = value;
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

            var numberOfMissingShipFields = targetNumberPerIndex
                .Zip(numberOfShipFields, (target, actual) => target - actual)
                .ToList();

            if (numberOfMissingShipFields.All((missing) => missing == 0))
            {
                return Satisfiability.SATISFIED;
            }

            return numberOfMissingShipFields
                .Zip(numberOfUndeterminedFields, (missing, undetermined) => (missing >= 0) && (missing <= undetermined))
                .All(satisfiable => satisfiable)
                ? Satisfiability.SATISFIABLE
                : Satisfiability.VIOLATED;
        }

        public IEnumerator<int> GetEnumerator()
        {
            return ((IEnumerable<int>)targetNumberPerIndex).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return targetNumberPerIndex.GetEnumerator();
        }
    }
}
