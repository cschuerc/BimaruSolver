using System;

namespace Bimaru.Interfaces
{
    /// <summary>
    /// An invalid Bimaru game is found where only valid are meaningful.
    /// </summary>
    public class InvalidBimaruGameException : Exception
    {
        public InvalidBimaruGameException()
        {
        }

        public InvalidBimaruGameException(string message) : base(message)
        {
        }

        public InvalidBimaruGameException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
