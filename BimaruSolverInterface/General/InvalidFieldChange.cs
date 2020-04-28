using System;

namespace BimaruInterfaces
{
    /// <summary>
    /// A field value is changed to an invalid value.
    /// </summary>
    public class InvalidFieldChange : InvalidBimaruGame
    {
        /// <inheritdoc/>
        public InvalidFieldChange() : base()
        {
        }

        /// <inheritdoc/>
        public InvalidFieldChange(string message) : base(message)
        {
        }

        /// <inheritdoc/>
        public InvalidFieldChange(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
