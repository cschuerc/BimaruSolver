using System;

namespace BimaruInterfaces
{
    /// <summary>
    /// Meta info about a Bimaru game
    /// </summary>
    public interface IGameMetaInfo : IEquatable<IGameMetaInfo>
    {
        /// <summary>
        /// Unique ID
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Size of the game
        /// </summary>
        GameSize Size { get; }

        /// <summary>
        /// Difficulty of the game
        /// </summary>
        GameDifficulty Difficulty { get; }
    }
}
