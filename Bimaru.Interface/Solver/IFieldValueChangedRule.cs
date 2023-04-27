using Bimaru.Interface.Game;

namespace Bimaru.Interface.Solver
{
    /// <summary>
    /// Solver rule that is supposed to be run after every field value change (e.g. UNDETERMINED to SHIP_SINGLE).
    /// </summary>
    public interface IFieldValueChangedRule
    {
        /// <summary>
        /// Solver rule that is supposed to be run after every field value change.
        /// </summary>
        void FieldValueChanged(IBimaruGame game, FieldValueChangedEventArgs<BimaruValue> e);
    }
}
