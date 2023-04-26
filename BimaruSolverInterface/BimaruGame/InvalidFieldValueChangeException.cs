using System;

namespace Bimaru.Interfaces
{
    /// <summary>
    /// A field value is changed to an invalid value.
    /// </summary>
    public class InvalidFieldValueChangeException : InvalidBimaruGameException
    {
        public InvalidFieldValueChangeException()
        {
        }

        public InvalidFieldValueChangeException(string message) : base(message)
        {
        }

        public InvalidFieldValueChangeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
