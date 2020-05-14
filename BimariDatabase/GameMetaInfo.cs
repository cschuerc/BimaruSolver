using BimaruInterfaces;
using System;

namespace BimaruDatabase
{
    /// <summary>
    /// Meta information about a Bimaru game.
    /// Equality based on all data fields.
    /// </summary>
    [Serializable]
    public class GameMetaInfo : IGameMetaInfo
    {
        public GameMetaInfo(int id, GameSize size, GameDifficulty difficulty)
        {
            ID = id;
            Size = size;
            Difficulty = difficulty;
        }

        public int ID
        {
            get;
            private set;
        }

        public GameSize Size
        {
            get;
            private set;
        }

        public GameDifficulty Difficulty
        {
            get;
            private set;
        }

        public static bool Equals(GameMetaInfo objLeft, IGameMetaInfo objRight)
        {
            if (objLeft == null && objRight == null)
            {
                return true;
            }

            if (objLeft == null)
            {
                return objRight.Equals(objLeft);
            }
            else
            {
                return objLeft.Equals(objRight);
            }
        }

        public override bool Equals(object other)
        {
            return Equals(other as IGameMetaInfo);
        }

        public bool Equals(IGameMetaInfo other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ID == other.ID && Size == other.Size && Difficulty == other.Difficulty;
        }

        public override int GetHashCode()
        {
            return (ID.GetHashCode() * 23 + Size.GetHashCode()) * 23 + Difficulty.GetHashCode();
        }

        public static bool operator ==(GameMetaInfo objLeft, IGameMetaInfo objRight)
        {
            return Equals(objLeft, objRight);
        }

        public static bool operator !=(GameMetaInfo objLeft, IGameMetaInfo objRight)
        {
            return !Equals(objLeft, objRight);
        }
    }
}
