﻿using System.Collections.Generic;

namespace BimaruInterfaces
{
    public enum BimaruValueConstraint
    {
        NO,

        /// <summary>
        /// Any kind of ship field.
        /// </summary>
        SHIP,

        WATER
    }

    public static class BimaruValueConstraints
    {
        private static readonly Dictionary<BimaruValueConstraint, BimaruValue> representativeValue =
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
        public static BimaruValue GetRepresentativeValue(this BimaruValueConstraint constraint)
        {
            return representativeValue[constraint];
        }

        /// <summary>
        /// True, if the constraint is satisfied by the Bimaru value.
        /// </summary>
        public static bool IsSatisfiedBy(this BimaruValueConstraint constraint, BimaruValue value)
        {
            return constraint == BimaruValueConstraint.NO || constraint == value.GetConstraint();
        }

        /// <summary>
        /// Whether the constraint allows the Bimaru value. Note that all
        /// constraints allow the value UNDETERMINED as the constraint
        /// can still be fullfilled in the future.
        /// </summary>
        public static bool DoesAllow(this BimaruValueConstraint constraint, BimaruValue value)
        {
            return constraint.IsSatisfiedBy(value) || value == BimaruValue.UNDETERMINED;
        }
    }
}