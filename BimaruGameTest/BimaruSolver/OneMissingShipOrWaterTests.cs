using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace BimaruSolver
{
    [TestClass]
    public class OneMissingShipOrWaterTests
    {
        private static Game SetupGame(int numRows, int numColumns)
        {
            var rowTally = new Tally(numRows);
            Tally columnTally = new Tally(numColumns);
            ShipSettings settings = new ShipSettings();
            RollbackGrid grid = new RollbackGrid(numRows, numColumns);

            return new Game(rowTally, columnTally, settings, grid);
        }

        private static void CheckCorrectTrialChanges(
            IEnumerable<SingleChange<BimaruValue>> expected,
            IEnumerable<FieldsToChange<BimaruValue>> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count());

            foreach (var changeExp in expected)
            {
                var changeActual = actual.FirstOrDefault(a => a.Count() == 1 && a.Contains(changeExp));
                Assert.IsNotNull(changeActual);
            }
        }

        [TestMethod]
        public void TestFullyDetermined()
        {
            int numRows = 1;
            int numColumns = 1;

            var game = SetupGame(numRows, numColumns);
            game.Grid.SetFieldValue(new GridPoint(0, 0), BimaruValue.SHIP_MIDDLE);
            var rule = new OneMissingShipOrWater(null);
            Assert.AreEqual(0, rule.GetCompleteChangeTrials(game).Count());
        }

        [TestMethod]
        public void TestOneMissingShip()
        {
            int numRows = 3;
            int numColumns = 4;

            var game = SetupGame(numRows, numColumns);
            game.Grid.SetFieldValue(new GridPoint(2, 3), BimaruValue.WATER);
            game.RowTally[0] = 2;
            game.RowTally[1] = 2;
            game.RowTally[2] = 1;
            game.ColumnTally[0] = 3;
            game.ColumnTally[1] = 0;
            game.ColumnTally[2] = 0;
            game.ColumnTally[3] = 2;

            //   3002
            //   ----
            // 1|???W
            // 2|????
            // 2|????
            // => The only row or column with one missing WATER or ship
            //    is row 2 with one missing ship.

            var rule = new OneMissingShipOrWater(null);

            var changesExpected = new FieldsToChange<BimaruValue>()
            {
                { new GridPoint(2, 0), BimaruValue.SHIP_UNDETERMINED },
                { new GridPoint(2, 1), BimaruValue.SHIP_UNDETERMINED },
                { new GridPoint(2, 2), BimaruValue.SHIP_UNDETERMINED },
            };

            CheckCorrectTrialChanges(changesExpected, rule.GetCompleteChangeTrials(game));
        }

        [TestMethod]
        public void TestOneMissingWater()
        {
            int numRows = 3;
            int numColumns = 4;

            var game = SetupGame(numRows, numColumns);
            game.Grid.SetFieldValue(new GridPoint(2, 3), BimaruValue.SHIP_UNDETERMINED);
            game.RowTally[0] = 2;
            game.RowTally[1] = 2;
            game.RowTally[2] = 3;
            game.ColumnTally[0] = 0;
            game.ColumnTally[1] = 3;
            game.ColumnTally[2] = 3;
            game.ColumnTally[3] = 1;

            //   0331
            //   ----
            // 3|???S
            // 2|????
            // 2|????
            // => The only row or column with one missing WATER or ship
            //    is row 2 with one missing WATER.

            var rule = new OneMissingShipOrWater(null);

            var changesExpected = new FieldsToChange<BimaruValue>()
            {
                { new GridPoint(2, 0), BimaruValue.WATER },
                { new GridPoint(2, 1), BimaruValue.WATER },
                { new GridPoint(2, 2), BimaruValue.WATER },
            };

            CheckCorrectTrialChanges(changesExpected, rule.GetCompleteChangeTrials(game));
        }

        protected class CountTrialCalls : ITrialAndErrorRule
        {
            public CountTrialCalls()
            {
                Count = 0;
            }

            public int Count { get; private set; }

            public bool AreTrialsDisjoint => false;

            public IEnumerable<FieldsToChange<BimaruValue>> GetCompleteChangeTrials(IGame game)
            {
                Count++;
                yield return null;
            }
        }

        [TestMethod]
        public void TestNeverOneMissing()
        {
            int numRows = 2;
            int numColumns = 4;

            var game = SetupGame(numRows, numColumns);
            game.RowTally[0] = 2;
            game.RowTally[1] = 2;
            game.ColumnTally[0] = 2;
            game.ColumnTally[1] = 0;
            game.ColumnTally[2] = 0;
            game.ColumnTally[3] = 2;

            //   2002
            //   ----
            // 2|????
            // 2|????
            // => No row or column with one missing WATER or ship.

            var rule = new OneMissingShipOrWater(null);

            // No fall-back rule but fall-back rule is needed.
            Assert.ThrowsException<InvalidOperationException>(() => rule.GetCompleteChangeTrials(game));

            // Check fall-back rule is called
            var counter = new CountTrialCalls();
            rule = new OneMissingShipOrWater(counter);

            int numTrials = 0;
            foreach (var changes in rule.GetCompleteChangeTrials(game))
            {
                Assert.IsNull(changes);
                numTrials++;
            }

            Assert.AreEqual(1, numTrials);
            Assert.AreEqual(1, counter.Count);
        }
    }
}
