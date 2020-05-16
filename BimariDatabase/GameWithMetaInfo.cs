using Bimaru.Interfaces;
using System;

namespace Bimaru.DatabaseUtil
{
    [Serializable]
    public class GameWithMetaInfo : IGameWithMetaInfo
    {
        public GameWithMetaInfo(IGameMetaInfo metaInfo, IGame game)
        {
            MetaInfo = metaInfo;
            Game = game;
        }


        private IGameMetaInfo _metaInfo;

        public IGameMetaInfo MetaInfo
        {
            get
            {
                return _metaInfo;
            }

            private set
            {
                _metaInfo = value ?? throw new ArgumentNullException("MetaInfo");
            }
        }


        private IGame game;

        public IGame Game
        {
            get
            {
                return game;
            }

            private set
            {
                game = value ?? throw new ArgumentNullException("Game");
            }
        }
    }
}
