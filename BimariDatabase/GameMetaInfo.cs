using BimaruInterfaces;
using System;

namespace BimaruDatabase
{
    /// <summary>
    /// Standard implementation of IDatabaseGame
    /// </summary>
    [Serializable]
    public class GameMetaInfo : IGameMetaInfo
    {
        /// <summary>
        /// Creates meta information about a Bimaru game
        /// </summary>
        /// <param name="id"> Unique identity </param>
        /// <param name="size"> Size of the game </param>
        /// <param name="difficulty"> Difficulty of the game </param>
        public GameMetaInfo(int id, GameSize size, GameDifficulty difficulty)
        {
            ID = id;
            Size = size;
            Difficulty = difficulty;
        }

        /// <inheritdoc/>
        public int ID { get; private set; }

        /// <inheritdoc/>
        public GameSize Size { get; private set; }

        /// <inheritdoc/>
        public GameDifficulty Difficulty { get; private set; }

        /// <summary>
        /// Equality based on data
        /// </summary>
        /// <param name="objLeft"></param>
        /// <param name="objRight"></param>
        /// <returns></returns>
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

        /// <inheritdoc/>
        public override bool Equals(object other)
        {
            return Equals(other as IGameMetaInfo);
        }

        /// <summary>
        /// Equality based on data
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
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

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return (ID.GetHashCode() * 23 + Size.GetHashCode()) * 23 + Difficulty.GetHashCode();
        }

        /// <summary>
        /// Equality based on data
        /// </summary>
        /// <param name="objLeft"></param>
        /// <param name="objRight"></param>
        /// <returns></returns>
        public static bool operator ==(GameMetaInfo objLeft, IGameMetaInfo objRight)
        {
            return Equals(objLeft, objRight);
        }

        /// <summary>
        /// Inequality based on data
        /// </summary>
        /// <param name="objLeft"></param>
        /// <param name="objRight"></param>
        /// <returns></returns>
        public static bool operator !=(GameMetaInfo objLeft, IGameMetaInfo objRight)
        {
            return !Equals(objLeft, objRight);
        }
    }
}
