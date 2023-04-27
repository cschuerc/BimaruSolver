using System;
using System.Collections.Generic;
using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Utility;

namespace Bimaru.Database.Generator
{
    public static class GameDatabaseGenerator
    {
        public static IEnumerable<IGameWithMetaInfo> GenerateGames()
        {
            return new List<IGameWithMetaInfo>()
            {
                GenerateDatabaseGame(
                    1,
                    GameSize.SMALL,
                    GameDifficulty.EASY,
                    new[] { 2, 1, 3, 1, 1, 2 },
                    new[] { 2, 1, 2, 2, 0, 3 },
                    new[] { 0, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(0, 1), BimaruValue.SHIP_CONT_RIGHT },
                        { new GridPoint(2, 2), BimaruValue.WATER },
                        { new GridPoint(5, 0), BimaruValue.WATER },
                        { new GridPoint(5, 5), BimaruValue.WATER },
                    }),

                GenerateDatabaseGame(
                    2,
                    GameSize.SMALL,
                    GameDifficulty.EASY,
                    new[] { 1, 2, 2, 1, 1, 3 },
                    new[] { 2, 2, 0, 2, 1, 3 },
                    new[] { 0, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(2, 4), BimaruValue.WATER },
                        { new GridPoint(4, 5), BimaruValue.SHIP_CONT_DOWN },
                    }),

                GenerateDatabaseGame(
                    3,
                    GameSize.MIDDLE,
                    GameDifficulty.EASY,
                    new[] { 4, 0, 4, 1, 1, 4, 1, 5 },
                    new[] { 5, 2, 2, 2, 2, 2, 2, 3 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(0, 2), BimaruValue.SHIP_MIDDLE },
                        { new GridPoint(2, 3), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(2, 7), BimaruValue.SHIP_CONT_UP },
                        { new GridPoint(5, 0), BimaruValue.SHIP_MIDDLE },
                        { new GridPoint(5, 6), BimaruValue.WATER },
                    }),

                GenerateDatabaseGame(
                    4,
                    GameSize.MIDDLE,
                    GameDifficulty.EASY,
                    new[] { 4, 2, 0, 3, 2, 3, 2, 4 },
                    new[] { 5, 0, 3, 4, 3, 1, 1, 3 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(5, 0), BimaruValue.WATER },
                        { new GridPoint(6, 6), BimaruValue.SHIP_SINGLE },
                    }),

                GenerateDatabaseGame(
                    5,
                    GameSize.LARGE,
                    GameDifficulty.EASY,
                    new[] { 1, 2, 1, 2, 3, 2, 2, 3, 1, 3 },
                    new[] { 2, 5, 2, 1, 0, 2, 1, 0, 4, 3 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(0, 0), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(1, 9), BimaruValue.WATER },
                        { new GridPoint(2, 8), BimaruValue.SHIP_CONT_UP },
                        { new GridPoint(7, 1), BimaruValue.SHIP_CONT_RIGHT },
                    }),

                GenerateDatabaseGame(
                    6,
                    GameSize.LARGE,
                    GameDifficulty.EASY,
                    new[] { 1, 3, 2, 2, 3, 1, 1, 1, 5, 1 },
                    new[] { 2, 3, 2, 0, 1, 2, 5, 0, 2, 3 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(1, 6), BimaruValue.WATER },
                        { new GridPoint(2, 2), BimaruValue.SHIP_CONT_DOWN },
                        { new GridPoint(7, 9), BimaruValue.SHIP_MIDDLE },
                    }),

                GenerateDatabaseGame(
                    7,
                    GameSize.LARGE,
                    GameDifficulty.EASY,
                    new[] { 3, 1, 3, 0, 3, 2, 4, 1, 2, 1 },
                    new[] { 2, 0, 4, 1, 2, 2, 2, 4, 1, 2 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(0, 7), BimaruValue.SHIP_MIDDLE },
                        { new GridPoint(2, 4), BimaruValue.SHIP_CONT_LEFT },
                        { new GridPoint(4, 2), BimaruValue.WATER },
                        { new GridPoint(4, 7), BimaruValue.WATER },
                        { new GridPoint(7, 2), BimaruValue.SHIP_CONT_UP },
                    }),

                GenerateDatabaseGame(
                    8,
                    GameSize.LARGE,
                    GameDifficulty.EASY,
                    new[] { 1, 1, 2, 2, 1, 3, 1, 2, 5, 2 },
                    new[] { 2, 3, 1, 0, 1, 5, 0, 5, 2, 1 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(1, 8), BimaruValue.SHIP_CONT_UP },
                        { new GridPoint(2, 1), BimaruValue.SHIP_CONT_UP },
                        { new GridPoint(8, 7), BimaruValue.WATER },
                    }),

                GenerateDatabaseGame(
                    9,
                    GameSize.MIDDLE,
                    GameDifficulty.MIDDLE,
                    new[] { 4, 1, 3, 2, 2, 3, 1, 4 },
                    new[] { 6, 1, 3, 2, 3, 0, 2, 3 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(0, 1), BimaruValue.SHIP_CONT_LEFT },
                        { new GridPoint(1, 6), BimaruValue.SHIP_CONT_UP },
                        { new GridPoint(2, 2), BimaruValue.WATER },
                        { new GridPoint(4, 6), BimaruValue.WATER },
                    }),

                GenerateDatabaseGame(
                    10,
                    GameSize.MIDDLE,
                    GameDifficulty.MIDDLE,
                    new[] { 2, 1, 4, 3, 2, 4, 1, 3 },
                    new[] { 4, 2, 1, 4, 1, 5, 1, 2 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(1, 1), BimaruValue.WATER },
                        { new GridPoint(1, 4), BimaruValue.WATER },
                        { new GridPoint(2, 0), BimaruValue.SHIP_CONT_RIGHT },
                        { new GridPoint(5, 0), BimaruValue.SHIP_CONT_UP },
                        { new GridPoint(5, 7), BimaruValue.WATER },
                    }),

                GenerateDatabaseGame(
                    11,
                    GameSize.LARGE,
                    GameDifficulty.MIDDLE,
                    new[] { 2, 1, 2, 1, 3, 3, 1, 3, 1, 3 },
                    new[] { 3, 2, 3, 1, 0, 4, 1, 2, 1, 3 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(1, 9), BimaruValue.SHIP_CONT_UP },
                        { new GridPoint(4, 0), BimaruValue.SHIP_CONT_UP },
                        { new GridPoint(4, 7), BimaruValue.WATER },
                        { new GridPoint(5, 9), BimaruValue.WATER },
                        { new GridPoint(8, 2), BimaruValue.SHIP_CONT_DOWN },
                    }),

                GenerateDatabaseGame(
                    12,
                    GameSize.LARGE,
                    GameDifficulty.MIDDLE,
                    new[] { 2, 2, 2, 1, 3, 3, 1, 4, 1, 1 },
                    new[] { 2, 2, 2, 3, 2, 3, 1, 1, 1, 3 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(0, 5), BimaruValue.SHIP_CONT_LEFT },
                        { new GridPoint(1, 9), BimaruValue.WATER },
                        { new GridPoint(2, 9), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(5, 7), BimaruValue.SHIP_MIDDLE },
                    }),

                GenerateDatabaseGame(
                    13,
                    GameSize.LARGE,
                    GameDifficulty.MIDDLE,
                    new[] { 2, 4, 0, 2, 4, 1, 1, 2, 2, 2 },
                    new[] { 1, 4, 1, 1, 3, 2, 3, 1, 3, 1 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(0, 6), BimaruValue.SHIP_CONT_LEFT },
                        { new GridPoint(3, 6), BimaruValue.WATER },
                        { new GridPoint(5, 2), BimaruValue.WATER },
                        { new GridPoint(7, 1), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(7, 8), BimaruValue.SHIP_MIDDLE },
                    }),

                GenerateDatabaseGame(
                    14,
                    GameSize.LARGE,
                    GameDifficulty.MIDDLE,
                    new[] { 1, 1, 3, 1, 2, 5, 1, 2, 1, 3 },
                    new[] { 1, 5, 1, 2, 2, 3, 1, 1, 2, 2 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(0, 1), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(2, 6), BimaruValue.SHIP_MIDDLE },
                        { new GridPoint(8, 9), BimaruValue.SHIP_CONT_UP },
                    }),

                GenerateDatabaseGame(
                    15,
                    GameSize.LARGE,
                    GameDifficulty.MIDDLE,
                    new[] { 4, 3, 1, 1, 1, 1, 3, 2, 3, 1 },
                    new[] { 1, 1, 1, 5, 1, 3, 2, 3, 1, 2 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(0, 2), BimaruValue.SHIP_CONT_RIGHT },
                        { new GridPoint(1, 0), BimaruValue.WATER },
                        { new GridPoint(4, 7), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(7, 9), BimaruValue.SHIP_CONT_UP },
                    }),

                GenerateDatabaseGame(
                    16,
                    GameSize.LARGE,
                    GameDifficulty.MIDDLE,
                    new[] { 3, 2, 1, 4, 2, 2, 1, 2, 1, 2 },
                    new[] { 1, 4, 2, 2, 1, 2, 1, 5, 1, 1 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(3, 6), BimaruValue.SHIP_MIDDLE },
                        { new GridPoint(7, 2), BimaruValue.SHIP_CONT_DOWN },
                        { new GridPoint(7, 9), BimaruValue.SHIP_SINGLE },
                    }),

                GenerateDatabaseGame(
                    17,
                    GameSize.LARGE,
                    GameDifficulty.HARD,
                    new[] { 1, 4, 1, 2, 2, 1, 5, 1, 2, 1 },
                    new[] { 1, 1, 1, 2, 3, 4, 1, 1, 4, 2 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(0, 1), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(1, 7), BimaruValue.SHIP_CONT_LEFT },
                        { new GridPoint(7, 0), BimaruValue.SHIP_SINGLE },
                    }),

                GenerateDatabaseGame(
                    18,
                    GameSize.LARGE,
                    GameDifficulty.HARD,
                    new[] { 1, 4, 1, 1, 6, 1, 2, 1, 1, 2 },
                    new[] { 1, 2, 3, 1, 2, 2, 4, 1, 3, 1 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(1, 2), BimaruValue.SHIP_CONT_UP },
                        { new GridPoint(1, 4), BimaruValue.WATER },
                        { new GridPoint(2, 7), BimaruValue.WATER },
                        { new GridPoint(7, 1), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(9, 5), BimaruValue.SHIP_CONT_RIGHT },
                    }),

                GenerateDatabaseGame(
                    19,
                    GameSize.LARGE,
                    GameDifficulty.HARD,
                    new[] { 2, 1, 1, 5, 2, 1, 3, 1, 2, 2 },
                    new[] { 2, 2, 3, 1, 1, 1, 2, 4, 2, 2 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(0, 5), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(5, 7), BimaruValue.WATER },
                        { new GridPoint(6, 1), BimaruValue.SHIP_CONT_RIGHT },
                        { new GridPoint(8, 6), BimaruValue.SHIP_CONT_DOWN },
                        { new GridPoint(9, 2), BimaruValue.WATER },
                    }),

                GenerateDatabaseGame(
                    20,
                    GameSize.LARGE,
                    GameDifficulty.HARD,
                    new[] { 2, 2, 5, 1, 3, 1, 3, 1, 1, 1 },
                    new[] { 4, 1, 2, 3, 1, 3, 1, 1, 1, 3 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(1, 8), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(5, 5), BimaruValue.SHIP_CONT_DOWN },
                        { new GridPoint(6, 0), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(7, 3), BimaruValue.WATER },
                        { new GridPoint(8, 5), BimaruValue.WATER },
                    }),

                GenerateDatabaseGame(
                    21,
                    GameSize.LARGE,
                    GameDifficulty.HARD,
                    new[] { 3, 1, 3, 2, 2, 1, 1, 1, 5, 1 },
                    new[] { 1, 2, 3, 3, 1, 2, 3, 2, 1, 2 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(1, 5), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(3, 0), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(4, 9), BimaruValue.SHIP_CONT_UP },
                    }),

                GenerateDatabaseGame(
                    22,
                    GameSize.LARGE,
                    GameDifficulty.HARD,
                    new[] { 4, 1, 1, 3, 1, 3, 1, 1, 3, 2 },
                    new[] { 1, 1, 3, 4, 2, 1, 1, 4, 1, 2 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(3, 2), BimaruValue.WATER },
                        { new GridPoint(5, 9), BimaruValue.SHIP_CONT_UP },
                        { new GridPoint(7, 4), BimaruValue.WATER },
                        { new GridPoint(8, 0), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(8, 3), BimaruValue.SHIP_CONT_UP },
                    }),

                GenerateDatabaseGame(
                    23,
                    GameSize.LARGE,
                    GameDifficulty.HARD,
                    new[] { 3, 1, 1, 5, 1, 2, 1, 3, 2, 1 },
                    new[] { 1, 4, 1, 1, 1, 1, 3, 1, 3, 4 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(0, 5), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(3, 2), BimaruValue.SHIP_SINGLE },
                        { new GridPoint(4, 9), BimaruValue.SHIP_MIDDLE },
                        { new GridPoint(6, 0), BimaruValue.WATER },
                        { new GridPoint(9, 9), BimaruValue.WATER },
                    }),

                GenerateDatabaseGame(
                    24,
                    GameSize.LARGE,
                    GameDifficulty.HARD,
                    new[] { 2, 1, 4, 2, 1, 2, 2, 1, 4, 1 },
                    new[] { 3, 2, 2, 2, 2, 1, 1, 3, 3, 1 },
                    new[] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(0, 3), BimaruValue.SHIP_CONT_RIGHT },
                        { new GridPoint(0, 6), BimaruValue.WATER },
                        { new GridPoint(3, 6), BimaruValue.WATER },
                        { new GridPoint(7, 0), BimaruValue.SHIP_CONT_DOWN },
                        { new GridPoint(8, 9), BimaruValue.SHIP_SINGLE },
                    })
            };
        }

        private static IGameWithMetaInfo GenerateDatabaseGame(
            int id,
            GameSize size,
            GameDifficulty difficulty,
            IReadOnlyList<int> rowTallyData,
            IReadOnlyList<int> columnTallyData,
            IReadOnlyList<int> shipSettingsData,
            IEnumerable<Tuple<GridPoint, BimaruValue>> initialFieldValues
            )
        {
            var metaInfo = new GameMetaInfo(id, size, difficulty);
            var game = GenerateGame(rowTallyData, columnTallyData, shipSettingsData, initialFieldValues);

            return new GameWithMetaInfo(metaInfo, game);
        }

        private static IGame GenerateGame(
            IReadOnlyList<int> rowTallyData,
            IReadOnlyList<int> columnTallyData,
            IReadOnlyList<int> shipSettingsData,
            IEnumerable<Tuple<GridPoint, BimaruValue>> initialFieldValues)
        {
            var game = (new GameFactory()).GenerateEmptyGame(rowTallyData.Count, columnTallyData.Count);

            CopyToTally(game.TargetNumberOfShipFieldsPerRow, rowTallyData);
            CopyToTally(game.TargetNumberOfShipFieldsPerColumn, columnTallyData);
            CopyToShipSettings(game.TargetNumberOfShipsPerLength, shipSettingsData);
            CopyToGrid(game.Grid, initialFieldValues);

            return game;
        }

        private static void CopyToTally(IGridTally tally, IReadOnlyList<int> tallyData)
        {
            for (var i = 0; i < tallyData.Count; i++)
            {
                tally[i] = tallyData[i];
            }
        }

        private static void CopyToShipSettings(IShipTarget settings, IReadOnlyList<int> shipSettingsData)
        {
            // Ignore shipSettingsData[0], as 0-length ships do not exist
            for (var i = 1; i < shipSettingsData.Count; i++)
            {
                settings[i] = shipSettingsData[i];
            }
        }

        private static void CopyToGrid(IBimaruGrid grid, IEnumerable<Tuple<GridPoint, BimaruValue>> initialFieldValues)
        {
            foreach (var c in initialFieldValues)
            {
                grid[c.Item1] = c.Item2;
            }
        }

        private sealed class TupleList<T1, T2> : List<Tuple<T1, T2>>
        {
#pragma warning disable S1144
            public void Add(T1 item, T2 item2)
#pragma warning restore S1144
            {
                Add(new Tuple<T1, T2>(item, item2));
            }
        }
    }
}
