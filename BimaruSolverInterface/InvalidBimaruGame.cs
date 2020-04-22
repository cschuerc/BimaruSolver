using System;

namespace BimaruInterfaces
{
    /// <summary>
    /// An invalid Bimaru game is found where only valid are meaningful.
    /// </summary>
    public class InvalidBimaruGame : Exception
    {
        /// <inheritdoc/>
        public InvalidBimaruGame() : base()
        {

        }

        /// <inheritdoc/>
        public InvalidBimaruGame(string message) : base(message)
        {

        }
    }
}
