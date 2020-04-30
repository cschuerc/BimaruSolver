using BimaruDatabase;
using BimaruGame;
using BimaruInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Utility;

namespace BimaruDatabaseGenerator
{
    class Program
    {
        public class TupleList<T1, T2> : List<Tuple<T1, T2>>
        {
            public void Add(T1 item, T2 item2)
            {
                Add(new Tuple<T1, T2>(item, item2));
            }
        }

        private static ITally GenerateTally(int[] tallyData)
        {
            var tally = new Tally(tallyData.Length);

            for (int i = 0; i < tallyData.Length; i++)
            {
                tally[i] = tallyData[i];
            }

            return tally;
        }

        private static IShipSettings GenerateShipSettings(int[] shipSettingsData)
        {
            var settings = new ShipSettings();

            // Ignore shipSettingsData[0], as 0-length ships do not exist
            for (int i = 1; i < shipSettingsData.Length; i++)
            {
                settings[i] = shipSettingsData[i];
            }

            return settings;
        }

        private static IRollbackGrid GenerateGrid(int numRows, int numColumns, IEnumerable<Tuple<GridPoint, BimaruValue>> initialFieldValues)
        {
            var grid = new Grid(numRows, numColumns);
            foreach (var c in initialFieldValues)
            {
                grid.SetFieldValue(c.Item1, c.Item2);
            }

            return new RollbackGrid(grid);
        }

        private static IGame GenerateGame(
            int[] rowTallyData,
            int[] columnTallyData,
            int[] shipSettingsData,
            IEnumerable<Tuple<GridPoint, BimaruValue>> initialFieldValues)
        {
            var rowTally = GenerateTally(rowTallyData);
            var columnTally = GenerateTally(columnTallyData);
            var shipSettings = GenerateShipSettings(shipSettingsData);
            var grid = GenerateGrid(rowTally.Length, columnTally.Length, initialFieldValues);

            return new Game(rowTally, columnTally, shipSettings, grid);
        }

        private static IDatabaseGame GenerateDatabaseGame(
            int ID,
            GameSize size,
            GameDifficulty difficulty,
            int[] rowTallyData,
            int[] columnTallyData,
            int[] shipSettingsData,
            IEnumerable<Tuple<GridPoint, BimaruValue>> initialFieldValues
            )
        {
            var metaInfo = new GameMetaInfo(ID, size, difficulty);
            var game = GenerateGame(rowTallyData, columnTallyData, shipSettingsData, initialFieldValues);

            return new DatabaseGame(metaInfo, game);
        }

        private static void SerializeGamesToFile(string databaseNameFormat, IEnumerable<IDatabaseGame> gameDatabase)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            foreach (var game in gameDatabase)
            {
                using (Stream fileStream = File.Create(string.Format(databaseNameFormat, game.MetaInfo.ID)))
                {

                    serializer.Serialize(fileStream, game);
                }
            }
        }

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                return;
            }

            string databaseNameFormat = args[0];

            var gameDatabase = new List<IDatabaseGame>()
            {
                GenerateDatabaseGame(
                    1,
                    GameSize.SMALL,
                    GameDifficulty.EASY,
                    new int[6] { 2, 1, 3, 1, 1, 2 },
                    new int[6] { 2, 1, 2, 2, 0, 3 },
                    new int[4] { 0, 3, 2, 1 },
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
                    new int[6] { 1, 2, 2, 1, 1, 3 },
                    new int[6] { 2, 2, 0, 2, 1, 3 },
                    new int[4] { 0, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(2, 4), BimaruValue.WATER },
                        { new GridPoint(4, 5), BimaruValue.SHIP_CONT_DOWN },
                    }),

                GenerateDatabaseGame(
                    3,
                    GameSize.MIDDLE,
                    GameDifficulty.EASY,
                    new int[8] { 4, 0, 4, 1, 1, 4, 1, 5 },
                    new int[8] { 5, 2, 2, 2, 2, 2, 2, 3 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[8] { 4, 2, 0, 3, 2, 3, 2, 4 },
                    new int[8] { 5, 0, 3, 4, 3, 1, 1, 3 },
                    new int[5] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(5, 0), BimaruValue.WATER },
                        { new GridPoint(6, 6), BimaruValue.SHIP_SINGLE },
                    }),

                GenerateDatabaseGame(
                    5,
                    GameSize.LARGE,
                    GameDifficulty.EASY,
                    new int[10] { 1, 2, 1, 2, 3, 2, 2, 3, 1, 3 },
                    new int[10] { 2, 5, 2, 1, 0, 2, 1, 0, 4, 3 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 1, 3, 2, 2, 3, 1, 1, 1, 5, 1 },
                    new int[10] { 2, 3, 2, 0, 1, 2, 5, 0, 2, 3 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 3, 1, 3, 0, 3, 2, 4, 1, 2, 1 },
                    new int[10] { 2, 0, 4, 1, 2, 2, 2, 4, 1, 2 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 1, 1, 2, 2, 1, 3, 1, 2, 5, 2 },
                    new int[10] { 2, 3, 1, 0, 1, 5, 0, 5, 2, 1 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[8] { 4, 1, 3, 2, 2, 3, 1, 4 },
                    new int[8] { 6, 1, 3, 2, 3, 0, 2, 3 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[8] { 2, 1, 4, 3, 2, 4, 1, 3 },
                    new int[8] { 4, 2, 1, 4, 1, 5, 1, 2 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 2, 1, 2, 1, 3, 3, 1, 3, 1, 3 },
                    new int[10] { 3, 2, 3, 1, 0, 4, 1, 2, 1, 3 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 2, 2, 2, 1, 3, 3, 1, 4, 1, 1 },
                    new int[10] { 2, 2, 2, 3, 2, 3, 1, 1, 1, 3 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 2, 4, 0, 2, 4, 1, 1, 2, 2, 2 },
                    new int[10] { 1, 4, 1, 1, 3, 2, 3, 1, 3, 1 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 1, 1, 3, 1, 2, 5, 1, 2, 1, 3 },
                    new int[10] { 1, 5, 1, 2, 2, 3, 1, 1, 2, 2 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 4, 3, 1, 1, 1, 1, 3, 2, 3, 1 },
                    new int[10] { 1, 1, 1, 5, 1, 3, 2, 3, 1, 2 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 3, 2, 1, 4, 2, 2, 1, 2, 1, 2 },
                    new int[10] { 1, 4, 2, 2, 1, 2, 1, 5, 1, 1 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 1, 4, 1, 2, 2, 1, 5, 1, 2, 1 },
                    new int[10] { 1, 1, 1, 2, 3, 4, 1, 1, 4, 2 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 1, 4, 1, 1, 6, 1, 2, 1, 1, 2 },
                    new int[10] { 1, 2, 3, 1, 2, 2, 4, 1, 3, 1 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 2, 1, 1, 5, 2, 1, 3, 1, 2, 2 },
                    new int[10] { 2, 2, 3, 1, 1, 1, 2, 4, 2, 2 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 2, 2, 5, 1, 3, 1, 3, 1, 1, 1 },
                    new int[10] { 4, 1, 2, 3, 1, 3, 1, 1, 1, 3 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 3, 1, 3, 2, 2, 1, 1, 1, 5, 1 },
                    new int[10] { 1, 2, 3, 3, 1, 2, 3, 2, 1, 2 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 4, 1, 1, 3, 1, 3, 1, 1, 3, 2 },
                    new int[10] { 1, 1, 3, 4, 2, 1, 1, 4, 1, 2 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 3, 1, 1, 5, 1, 2, 1, 3, 2, 1 },
                    new int[10] { 1, 4, 1, 1, 1, 1, 3, 1, 3, 4 },
                    new int[5] { 0, 4, 3, 2, 1 },
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
                    new int[10] { 2, 1, 4, 2, 1, 2, 2, 1, 4, 1 },
                    new int[10] { 3, 2, 2, 2, 2, 1, 1, 3, 3, 1 },
                    new int[5] { 0, 4, 3, 2, 1 },
                    new TupleList<GridPoint, BimaruValue>()
                    {
                        { new GridPoint(0, 3), BimaruValue.SHIP_CONT_RIGHT },
                        { new GridPoint(0, 6), BimaruValue.WATER },
                        { new GridPoint(2, 6), BimaruValue.WATER },
                        { new GridPoint(7, 0), BimaruValue.SHIP_CONT_DOWN },
                        { new GridPoint(8, 9), BimaruValue.SHIP_SINGLE },
                    })
            };

            SerializeGamesToFile(databaseNameFormat, gameDatabase);
        }
    }
}
