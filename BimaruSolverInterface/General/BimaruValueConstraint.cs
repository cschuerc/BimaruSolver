using System.Collections.Generic;

namespace BimaruInterfaces
{
    /// <summary>
    /// Constraints of Bimaru field values
    /// </summary>
    public enum BimaruValueConstraint
    {
        /// <summary>
        /// No constraint.
        /// </summary>
        NO,

        /// <summary>
        /// Any kind of ship field.
        /// </summary>
        SHIP,

        /// <summary>
        /// Water
        /// </summary>
        WATER
    }

    /// <summary>
    /// Extensions to the ship boundaries
    /// </summary>
    public static class BimaruValueConstraintExtensions
    {
        private static readonly Dictionary<BimaruValueConstraint, BimaruValue> _representativeValue =
            new Dictionary<BimaruValueConstraint, BimaruValue>()
            {
                { BimaruValueConstraint.WATER, BimaruValue.WATER },
                { BimaruValueConstraint.SHIP, BimaruValue.SHIP_UNDETERMINED },
                { BimaruValueConstraint.NO, BimaruValue.UNDETERMINED },
            };

        /// <summary>
        /// Get a representative value of the constraint.
        /// The value fullfills the constraint without assuming more.
        /// </summary>
        /// <param name="constraint"> Constraint whose value is returned. </param>
        /// <returns> Representative value of the constraint. </returns>
        public static BimaruValue GetRepresentativeValue(this BimaruValueConstraint constraint)
        {
            return _representativeValue[constraint];
        }

        /// <summary>
        /// Whether the constraint is satisfied by the Bimaru value.
        /// </summary>
        /// <param name="constraint"> Constraint to be satisfied </param>
        /// <param name="value"> Bimaru value </param>
        /// <returns> True, if the value satisfies the constraint. </returns>
        public static bool IsSatisfiedBy(this BimaruValueConstraint constraint, BimaruValue value)
        {
            return constraint == BimaruValueConstraint.NO || constraint == value.GetConstraint();
        }

        /// <summary>
        /// Whether the constraint allows the field value. Note that all
        /// constraints allow the value UNDETERMINED as the constraint
        /// can still be fullfilled in the future.
        /// </summary>
        /// <param name="constraint"> Constraint </param>
        /// <param name="value"> Bimaru value </param>
        /// <returns> True, if the constraint allows the field value. </returns>
        public static bool DoesAllow(this BimaruValueConstraint constraint, BimaruValue value)
        {
            return constraint.IsSatisfiedBy(value) || value == BimaruValue.UNDETERMINED;
        }
    }
}