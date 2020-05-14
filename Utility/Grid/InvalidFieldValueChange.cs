using System;

namespace Utility
{
    /// <summary>
    /// A field value is changed to an invalid value.
    /// </summary>
    public class InvalidFieldValueChange : Exception
    {
        public InvalidFieldValueChange() : base()
        {
        }

        public InvalidFieldValueChange(string message) : base(message)
        {
        }

        public InvalidFieldValueChange(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
