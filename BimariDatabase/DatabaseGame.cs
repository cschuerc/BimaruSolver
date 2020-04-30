using BimaruInterfaces;
using System;

namespace BimaruDatabase
{
    /// <summary>
    /// Standard implementation of IDatabaseGame
    /// </summary>
    [Serializable]
    public class DatabaseGame : IDatabaseGame
    {
        /// <summary>
        /// Creates a database game
        /// </summary>
        /// <param name="metaInfo"> Meta information </param>
        /// <param name="game"> Bimaru game </param>
        public DatabaseGame(IGameMetaInfo metaInfo, IGame game)
        {
            MetaInfo = metaInfo;
            Game = game;
        }


        private IGameMetaInfo _metaInfo;

        /// <inheritdoc/>
        public IGameMetaInfo MetaInfo
        {
            get
            {
                return _metaInfo;
            }

            private set
            {
                _metaInfo = value ?? throw new ArgumentNullException();
            }
        }


        private IGame _game;

        /// <inheritdoc/>
        public IGame Game
        {
            get
            {
                return _game;
            }

            private set
            {
                _game = value ?? throw new ArgumentNullException();
            }
        }
    }
}
