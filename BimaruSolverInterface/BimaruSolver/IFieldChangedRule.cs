using Utility;

namespace BimaruInterfaces
{
    /// <summary>
    /// Rule that is run after every field value change (i.e. UNDETERMINED to SHIP_SINGLE).
    /// </summary>
    public interface IFieldChangedRule
    {
        /// <summary>
        /// Rule to be run after a field value change.
        /// </summary>
        /// <param name="game"> Bimaru game </param>
        /// <param name="e"> Event arguments of the field value change. </param>
        void FieldValueChanged(IGame game, FieldValueChangedEventArgs<BimaruValue> e);
    }
}
