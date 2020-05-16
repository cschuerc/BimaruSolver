using System;

namespace Bimaru.Interfaces
{
    /// <summary>
    /// An invalid Bimaru game is found where only valid are meaningful.
    /// </summary>
    public class InvalidBimaruGame : Exception
    {
        public InvalidBimaruGame() : base()
        {
        }

        public InvalidBimaruGame(string message) : base(message)
        {
        }

        public InvalidBimaruGame(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
