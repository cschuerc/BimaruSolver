using System;

namespace Bimaru.Interfaces
{
    public interface IGameMetaInfo : IEquatable<IGameMetaInfo>
    {
        /// <summary>
        /// Unique identity
        /// </summary>
        int Id { get; }

        GameSize Size { get; }

        GameDifficulty Difficulty { get; }
    }
}
