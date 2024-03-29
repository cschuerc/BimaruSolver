﻿using Bimaru.Interface.Game;

namespace Bimaru.Interface.Solver
{
    public interface IBimaruSolver
    {
        /// <summary>
        /// Tries to solve the Bimaru.
        /// 
        /// Leaves the Bimaru game untouched
        /// if no solution could be found.
        /// 
        /// The state of the Bimaru is an arbitrary solution
        /// if at least one solution could be found.
        /// </summary>
        /// <returns> Number of found solutions. </returns>
        int Solve(IBimaruGame game);
    }
}
