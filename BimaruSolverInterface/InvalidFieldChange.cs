using System;

namespace BimaruInterfaces
{
    /// <summary>
    /// A field is changed to an invalid value.
    /// </summary>
    public class InvalidFieldChange : Exception
    {
        /// <inheritdoc/>
        public InvalidFieldChange() : base()
        {

        }

        /// <inheritdoc/>
        public InvalidFieldChange(string message) : base(message)
        {

        }
    }
}
