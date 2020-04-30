namespace BimaruInterfaces
{
    /// <summary>
    /// Bimaru game from a database with meta information
    /// </summary>
    public interface IDatabaseGame
    {
        /// <summary>
        /// Meta information about the game
        /// </summary>
        IGameMetaInfo MetaInfo { get; }

        /// <summary>
        /// Bimaru game
        /// </summary>
        IGame Game { get; }
    }
}
