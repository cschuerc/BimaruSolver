using System;

namespace Bimaru.Interface.Database
{
    /// <summary>
    /// Meta information about a Bimaru game.
    /// Equality based on all data fields.
    /// </summary>
    [Serializable]
    public sealed class GameMetaInfo
    {
        public GameMetaInfo(int id, GameSize size, GameDifficulty difficulty)
        {
            Id = id;
            Size = size;
            Difficulty = difficulty;
        }

        public int Id
        {
            get;
        }

        public GameSize Size
        {
            get;
        }

        public GameDifficulty Difficulty
        {
            get;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as GameMetaInfo);
        }

        public bool Equals(GameMetaInfo other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id == other.Id && Size == other.Size && Difficulty == other.Difficulty;
        }

        public override int GetHashCode()
        {
            return (Id.GetHashCode() * 23 + Size.GetHashCode()) * 23 + Difficulty.GetHashCode();
        }
    }
}
