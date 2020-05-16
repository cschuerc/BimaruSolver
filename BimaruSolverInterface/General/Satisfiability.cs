namespace Bimaru.Interfaces
{
    public enum Satisfiability
    {
        VIOLATED,

        /// <summary>
        /// The constraint is not (yet) satisfied but also not violated
        /// </summary>
        SATISFIABLE,

        SATISFIED
    }
}
