using System;

namespace BimaruInterfaces
{
    public interface IGameMetaInfo : IEquatable<IGameMetaInfo>
    {
        /// <summary>
        /// Unique identity
        /// </summary>
        int ID { get; }

        GameSize Size { get; }

        GameDifficulty Difficulty { get; }
    }
}
