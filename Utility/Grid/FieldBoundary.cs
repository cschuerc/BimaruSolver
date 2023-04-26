using System.Collections.Generic;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Utility
{
    /// <summary>
    /// Boundaries of two neighboring (also diagonally) fields.
    /// Each field on the grid has eight possible boundaries,
    /// for each possible direction one.
    /// </summary>
    public struct FieldBoundary
    {
        /// <summary>
        /// Subset of directions such that the boundary representation is unique.
        /// A boundary (0, 0) UP is the same as (1, 0) DOWN. Hence,
        /// UP and DOWN should not both be used, otherwise the same boundary
        /// is represented in differing structs. The same is also true for:
        /// RIGHT_UP and LEFT_DOWN
        /// RIGHT and LEFT
        /// RIGHT_DOWN and LEFT_UP
        /// </summary>
        private static readonly HashSet<Direction> baseDirections =
            new HashSet<Direction>()
            {
                Direction.UP,
                Direction.RIGHT_UP,
                Direction.RIGHT,
                Direction.RIGHT_DOWN
            };

        /// <summary>
        /// Field boundary with a unique representation. For example,
        /// if this is called once with (0, 0) UP and once with (1, 0) DOWN
        /// then both represent the same boundary and are equal.
        /// </summary>
        /// <param name="basePoint"> Point which has the boundary </param>
        /// <param name="directionFromBase"> Direction of the boundary relative to the base point </param>
        public FieldBoundary(GridPoint basePoint, Direction directionFromBase)
        {
            // Unique representation for the same boundary
            if (!baseDirections.Contains(directionFromBase))
            {
                basePoint = basePoint.GetNextPoint(directionFromBase);
                directionFromBase = directionFromBase.GetOpposite();
            }

            BasePoint = basePoint;
            DirectionFromBase = directionFromBase;
        }

        /// <summary>
        /// The base point of the boundary. Is unambiguous,
        /// so for example for the boundary between (0, 0)
        /// and (1, 0), the base point is always (0, 0).
        /// </summary>
        public GridPoint BasePoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Direction of the boundary relative to the base point.
        /// </summary>
        public Direction DirectionFromBase
        {
            get;
            private set;
        }
    }
}
