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
