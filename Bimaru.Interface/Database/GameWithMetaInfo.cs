using System;
using Bimaru.Interface.Game;

namespace Bimaru.Interface.Database
{
    [Serializable]
    public class GameWithMetaInfo
    {
        public GameWithMetaInfo(GameMetaInfo metaInfo, IBimaruGame game)
        {
            MetaInfo = metaInfo;
            Game = game;
        }

        public GameMetaInfo MetaInfo { get; }

        private IBimaruGame game;

        public IBimaruGame Game
        {
            get => game;

            private set => game = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
