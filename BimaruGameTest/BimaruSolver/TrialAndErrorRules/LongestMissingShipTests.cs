using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace BimaruSolver
{
    [TestClass]
    public class LongestMissingShipTests
    {
        private static FieldsToChange<BimaruValue> GenChangesShip(GridPoint p, Direction direction, int shipLength)
        {
            var values = BimaruValues.FieldValuesOfShip(direction, shipLength);
            return new FieldsToChange<BimaruValue>(p, direction, values);
        }

        private static void CheckEqualChanges(FieldsToChange<BimaruValue> first, FieldsToChange<BimaruValue> second)
        {
            Assert.AreEqual(first.Count(), second.Count());

            // FieldsToChange contains by design no duplicate SingleChange
            // => Same count and first contained in second is enough for equality
            foreach (var c in first)
            {
                Assert.IsTrue(second.Contains(c));
            }
        }

        private static void CheckCorrectTrialChanges(
            Dictionary<SingleChange<BimaruValue>, FieldsToChange<BimaruValue>> expected,
            IEnumerable<FieldsToChange<BimaruValue>> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count());

            foreach (var changesExp in expected)
            {
                var changesActual = actual.FirstOrDefault(a => a.Contains(changesExp.Key));

                Assert.IsNotNull(changesActual);
                CheckEqualChanges(changesActual, changesExp.Value);
            }
        }

        [TestMethod]
        public void TestFullyDetermined()
        {
            var game = (new GameFactory()).GenerateEmptyGame(1, 1);
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_MIDDLE;
            var rule = new LongestMissingShip();
            Assert.AreEqual(0, rule.GetCompleteChangeTrials(game).Count());
        }

        [TestMethod]
        public void TestBasic()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            game.TargetNumberOfShipFieldsPerRow[1] = 1;
            game.TargetNumberOfShipFieldsPerRow[2] = 2;

            game.TargetNumberOfShipFieldsPerColumn[1] = 1;
            game.TargetNumberOfShipFieldsPerColumn[2] = 2;

            game.TargetNumberOfShipsPerLength[2] = 1;

            game.Grid[new GridPoint(0, 0)] = BimaruValue.WATER;
            game.Grid[new GridPoint(1, 0)] = BimaruValue.WATER;
            game.Grid[new GridPoint(2, 0)] = BimaruValue.WATER;
            game.Grid[new GridPoint(0, 1)] = BimaruValue.WATER;
            game.Grid[new GridPoint(1, 1)] = BimaruValue.WATER;
            game.Grid[new GridPoint(0, 2)] = BimaruValue.WATER;

            // 1xDESTROYER
            //   012
            //   ---
            // 2|W??
            // 1|WW?
            // 0|WWW

            var rule = new LongestMissingShip();

            
            int shipLength = 2;
            var startChange0 = new SingleChange<BimaruValue>(new GridPoint(2, 1), BimaruValue.SHIP_CONT_RIGHT);
            var startChange1 = new SingleChange<BimaruValue>(new GridPoint(1, 2), BimaruValue.SHIP_CONT_UP);

            // Key is a representative SingleChange that is only present in one FieldsToChange
            var expectedChanges = new Dictionary<SingleChange<BimaruValue>, FieldsToChange<BimaruValue>>
            {
                {startChange0, GenChangesShip(startChange0.Point, Direction.RIGHT, shipLength) },
                {startChange1, GenChangesShip(startChange1.Point, Direction.UP, shipLength) }
            };

            CheckCorrectTrialChanges(expectedChanges, rule.GetCompleteChangeTrials(game));
        }

        [TestMethod]
        public void TestEmptyGrid()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            game.TargetNumberOfShipFieldsPerRow[0] = 3;
            game.TargetNumberOfShipFieldsPerRow[1] = 2;
            game.TargetNumberOfShipFieldsPerRow[2] = 3;

            game.TargetNumberOfShipFieldsPerColumn[0] = 4;
            game.TargetNumberOfShipFieldsPerColumn[1] = 3;
            game.TargetNumberOfShipFieldsPerColumn[2] = 2;

            game.TargetNumberOfShipsPerLength[3] = 1;

            // 1xCRUISER
            //   432
            //   ---
            // 3|???
            // 2|???
            // 3|???

            var rule = new LongestMissingShip();


            int shipLength = 3;
            var startChange0 = new SingleChange<BimaruValue>(new GridPoint(2, 2), BimaruValue.SHIP_CONT_LEFT);
            var startChange1 = new SingleChange<BimaruValue>(new GridPoint(0, 2), BimaruValue.SHIP_CONT_LEFT);
            var startChange2 = new SingleChange<BimaruValue>(new GridPoint(1, 0), BimaruValue.SHIP_MIDDLE);
            var startChange3 = new SingleChange<BimaruValue>(new GridPoint(1, 1), BimaruValue.SHIP_MIDDLE);

            // Key is a representative SingleChange that is only present in one FieldsToChange
            var expectedChanges = new Dictionary<SingleChange<BimaruValue>, FieldsToChange<BimaruValue>>
            {
                {startChange0, GenChangesShip(startChange0.Point, Direction.LEFT, shipLength) },
                {startChange1, GenChangesShip(startChange1.Point, Direction.LEFT, shipLength) },
                {startChange2, GenChangesShip(new GridPoint(0, 0), Direction.UP, shipLength) },
                {startChange3, GenChangesShip(new GridPoint(0, 1), Direction.UP, shipLength) }
            };

            CheckCorrectTrialChanges(expectedChanges, rule.GetCompleteChangeTrials(game));
        }

        [TestMethod]
        public void TestSingleShip()
        {
            var game = (new GameFactory()).GenerateEmptyGame(2, 2);
            game.TargetNumberOfShipFieldsPerRow[0] = 1;
            game.TargetNumberOfShipFieldsPerRow[1] = 1;

            game.TargetNumberOfShipFieldsPerColumn[0] = 1;
            game.TargetNumberOfShipFieldsPerColumn[1] = 1;

            game.TargetNumberOfShipsPerLength[1] = 1;

            // 1xSUBMARINE
            //   11
            //   --
            // 1|??
            // 1|??

            var rule = new LongestMissingShip();


            var startChange0 = new SingleChange<BimaruValue>(new GridPoint(0, 0), BimaruValue.SHIP_SINGLE);
            var startChange1 = new SingleChange<BimaruValue>(new GridPoint(0, 1), BimaruValue.SHIP_SINGLE);
            var startChange2 = new SingleChange<BimaruValue>(new GridPoint(1, 0), BimaruValue.SHIP_SINGLE);
            var startChange3 = new SingleChange<BimaruValue>(new GridPoint(1, 1), BimaruValue.SHIP_SINGLE);

            // Key is a representative SingleChange that is only present in one FieldsToChange
            var expectedChanges = new Dictionary<SingleChange<BimaruValue>, FieldsToChange<BimaruValue>>
            {
                {startChange0, new FieldsToChange<BimaruValue>(startChange0.Point, BimaruValue.SHIP_SINGLE) },
                {startChange1, new FieldsToChange<BimaruValue>(startChange1.Point, BimaruValue.SHIP_SINGLE) },
                {startChange2, new FieldsToChange<BimaruValue>(startChange2.Point, BimaruValue.SHIP_SINGLE) },
                {startChange3, new FieldsToChange<BimaruValue>(startChange3.Point, BimaruValue.SHIP_SINGLE) },
            };

            CheckCorrectTrialChanges(expectedChanges, rule.GetCompleteChangeTrials(game));
        }

        [TestMethod]
        public void TestPreset()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            game.TargetNumberOfShipFieldsPerRow[0] = 2;
            game.TargetNumberOfShipFieldsPerRow[1] = 1;
            game.TargetNumberOfShipFieldsPerRow[2] = 2;

            game.TargetNumberOfShipFieldsPerColumn[0] = 2;
            game.TargetNumberOfShipFieldsPerColumn[1] = 1;
            game.TargetNumberOfShipFieldsPerColumn[2] = 2;

            game.TargetNumberOfShipsPerLength[2] = 1;

            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_CONT_UP;
            game.Grid[new GridPoint(2, 0)] = BimaruValue.SHIP_MIDDLE;
            game.Grid[new GridPoint(0, 2)] = BimaruValue.WATER;
            game.Grid[new GridPoint(2, 2)] = BimaruValue.SHIP_UNDETERMINED;

            // 1xDESTROYER
            //   212
            //   ---
            // 2|S?S
            // 1|???
            // 2|S?W

            var rule = new LongestMissingShip();


            int shipLength = 2;
            var startChange0 = new SingleChange<BimaruValue>(new GridPoint(0, 0), BimaruValue.SHIP_CONT_UP);
            var startChange1 = new SingleChange<BimaruValue>(new GridPoint(2, 1), BimaruValue.SHIP_CONT_RIGHT);
            var startChange2 = new SingleChange<BimaruValue>(new GridPoint(1, 2), BimaruValue.SHIP_CONT_UP);

            // Key is a representative SingleChange that is only present in one FieldsToChange
            var expectedChanges = new Dictionary<SingleChange<BimaruValue>, FieldsToChange<BimaruValue>>
            {
                {startChange0, GenChangesShip(startChange0.Point, Direction.UP, shipLength) },
                {startChange1, GenChangesShip(startChange1.Point, Direction.RIGHT, shipLength) },
                {startChange2, GenChangesShip(startChange2.Point, Direction.UP, shipLength) }
            };

            CheckCorrectTrialChanges(expectedChanges, rule.GetCompleteChangeTrials(game));
        }

        [TestMethod]
        public void TestShipLength()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            game.TargetNumberOfShipFieldsPerRow[0] = 1;
            game.TargetNumberOfShipFieldsPerRow[1] = 2;
            game.TargetNumberOfShipFieldsPerRow[2] = 2;

            game.TargetNumberOfShipFieldsPerColumn[0] = 2;
            game.TargetNumberOfShipFieldsPerColumn[1] = 0;
            game.TargetNumberOfShipFieldsPerColumn[2] = 3;

            game.TargetNumberOfShipsPerLength[2] = 1;
            game.TargetNumberOfShipsPerLength[3] = 1;

            game.Grid[new GridPoint(0, 2)] = BimaruValue.SHIP_CONT_UP;
            game.Grid[new GridPoint(1, 2)] = BimaruValue.SHIP_MIDDLE;
            game.Grid[new GridPoint(2, 2)] = BimaruValue.SHIP_CONT_DOWN;

            // 1xDESTROYER, 1xCRUISER
            //   203
            //   ---
            // 2|??S
            // 2|??S
            // 1|??S

            var rule = new LongestMissingShip();


            int shipLength = 2;
            var startChange0 = new SingleChange<BimaruValue>(new GridPoint(0, 0), BimaruValue.SHIP_CONT_UP);
            var startChange1 = new SingleChange<BimaruValue>(new GridPoint(2, 0), BimaruValue.SHIP_CONT_DOWN);

            // Could be avoided in a more efficient implementation
            var startChange2 = new SingleChange<BimaruValue>(new GridPoint(1, 1), BimaruValue.SHIP_CONT_LEFT);
            var startChange3 = new SingleChange<BimaruValue>(new GridPoint(2, 1), BimaruValue.SHIP_CONT_LEFT);

            // Key is a representative SingleChange that is only present in one FieldsToChange
            var expectedChanges = new Dictionary<SingleChange<BimaruValue>, FieldsToChange<BimaruValue>>
            {
                {startChange0, GenChangesShip(startChange0.Point, Direction.UP, shipLength) },
                {startChange1, GenChangesShip(startChange1.Point, Direction.DOWN, shipLength) },
                {startChange2, GenChangesShip(startChange2.Point, Direction.LEFT, shipLength) },
                {startChange3, GenChangesShip(startChange3.Point, Direction.LEFT, shipLength) }
            };

            CheckCorrectTrialChanges(expectedChanges, rule.GetCompleteChangeTrials(game));
        }
    }
}
