﻿namespace Bimaru.Interface.Utility
{
    public enum Satisfiability
    {
        VIOLATED,

        /// <summary>
        /// The constraint is not (yet) satisfied but also not violated
        /// </summary>
        SATISFIABLE,

        SATISFIED
    }
}
