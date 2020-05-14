using System.Collections.Generic;

namespace BimaruInterfaces
{
    /// <summary>
    /// Target number of fields per row/column to satisfy a condition.
    /// </summary>
    public interface IGridTally: IEnumerable<int>
    {
        /// <summary>
        /// Number of rows/columns
        /// </summary>
        int Length
        {
            get;
        }

        /// <summary>
        /// Target number of fields per row/column to satisfy a condition.
        /// </summary>
        /// <param name="index"> Row/column index. Upper bounded by Length. </param>
        /// <returns></returns>
        int this[int index]
        {
            get;
            set;
        }

        /// <summary>
        /// Total target number of fields over all rows/columns
        /// </summary>
        int Total
        {
            get;
        }

        /// <summary>
        /// How the target number of ship fields is satisfied.
        /// </summary>
        /// <param name="numberOfFields"> Real number of fields per row/column that satisfy the condition. </param>
        /// <param name="additionalNumberOfFields"> Additional number of fields per row/column that don't satisfy the condition but could </param>
        Satisfiability GetSatisfiability(
            IReadOnlyList<int> numberOfFields,
            IReadOnlyList<int> additionalNumberOfFields);
    }
}
