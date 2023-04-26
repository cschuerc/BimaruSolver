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


        private IGameMetaInfo metaInfo;

        public IGameMetaInfo MetaInfo
        {
            get => metaInfo;

            private set => metaInfo = value ?? throw new ArgumentNullException(nameof(value));
        }


        private IGame game;

        public IGame Game
        {
            get => game;

            private set => game = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
