using System;
using Bimaru.Interfaces;

namespace Bimaru.Database
{
    [Serializable]
    public class GameWithMetaInfo : IGameWithMetaInfo
    {
        public GameWithMetaInfo(IGameMetaInfo metaInfo, IBimaruGame game)
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


        private IBimaruGame game;

        public IBimaruGame Game
        {
            get => game;

            private set => game = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
