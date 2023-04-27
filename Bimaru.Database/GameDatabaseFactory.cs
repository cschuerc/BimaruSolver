﻿using Bimaru.Interface.Database;

namespace Bimaru.Database
{
    public static class GameDatabaseFactory
    {
        public static IGameDatabase GetDatabase()
        {
            var gameSource = new GameSourceFromResources();
            return new GameDatabase(gameSource);
        }
    }
}