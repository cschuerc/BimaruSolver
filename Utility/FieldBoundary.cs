using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    /// Boundaries of two neighbouring (also diagonally) fields.
    /// Each field on the grid has eight possible boundaries.
    /// For each possible direction one.
    /// </summary>
    public struct FieldBoundary: IFieldBoundary
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
        private static readonly HashSet<Directions> _baseDirections =
            new HashSet<Directions>()
            {
                Directions.UP,
                Directions.RIGHT_UP,
                Directions.RIGHT,
                Directions.RIGHT_DOWN
            };
            
        private readonly IGridPoint _basePoint;

        private readonly Directions _boundaryDirectionFromBase;

        /// <summary>
        /// Constructs a field boundary with a unique representation
        /// </summary>
        /// <param name="basePoint"></param>
        /// <param name="direction"></param>
        public FieldBoundary(IGridPoint basePoint, Directions direction)
        {
            // Unique representation for the same boundary
            if (!_baseDirections.Contains(direction))
            {
                basePoint = basePoint.GetNextPoint(direction);
                direction = direction.GetOpposite();
            }

            _basePoint = basePoint;
            _boundaryDirectionFromBase = direction;
        }
    }
}
