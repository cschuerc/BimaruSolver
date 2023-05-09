using System.Collections.Generic;
using System.Linq;
using Bimaru.Interface.Utility;
using Bimaru.Solver;
using Bimaru.Solver.TrialAndErrorRules;
using Xunit;

namespace Bimaru.Tests.Solver.TrialAndErrorRules
{
    public class LongestMissingShipTests
    {
        [Fact]
        public void TestAreTrialsDisjointComplete()
        {
            var rule = new LongestMissingShip();

            Assert.False(rule.AreTrialsDisjoint);
            Assert.True(rule.AreTrialsComplete);
        }

        [Fact]
        public void TestFullyDetermined()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(1, 1);
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_MIDDLE;

            var rule = new LongestMissingShip();
            Assert.Empty(rule.GetChangeTrials(game));
        }

        [Fact]
        public void TestBasic()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(3, 3);
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

            const int shipLength = 2;
            var startChange0 = new SingleChange<BimaruValue>(new GridPoint(2, 1), BimaruValue.SHIP_CONT_RIGHT);
            var startChange1 = new SingleChange<BimaruValue>(new GridPoint(1, 2), BimaruValue.SHIP_CONT_UP);

            // Key is a representative SingleChange that is only present in one FieldsToChange
            var expectedChanges = new Dictionary<SingleChange<BimaruValue>, FieldsToChange<BimaruValue>>
            {
                {startChange0, new ShipLocation(startChange0.Point, Direction.RIGHT, shipLength).Changes },
                {startChange1, new ShipLocation(startChange1.Point, Direction.UP, shipLength).Changes }
            };

            AssertEqualTrialChanges(expectedChanges, rule.GetChangeTrials(game));
        }

        private static void AssertEqualTrialChanges(
            Dictionary<SingleChange<BimaruValue>, FieldsToChange<BimaruValue>> expected,
            IEnumerable<FieldsToChange<BimaruValue>> actual)
        {
            var actualAsList = actual.ToList();
            Assert.Equal(expected.Count, actualAsList.Count);

            foreach (var expectedChanges in expected)
            {
                var actualChanges = actualAsList.FirstOrDefault(a => a.Contains(expectedChanges.Key));

                Assert.NotNull(actualChanges);
                AssertEqualChanges(actualChanges, expectedChanges.Value);
            }
        }

        private static void AssertEqualChanges(FieldsToChange<BimaruValue> first, FieldsToChange<BimaruValue> second)
        {
            Assert.Equal(first.Count, second.Count);

            // FieldsToChange contains by design no duplicate field
            // => Same count + first contained in second is enough for equality
            foreach (var c in first)
            {
                Assert.Contains(c, second);
            }
        }

        [Fact]
        public void TestEmptyGrid()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(3, 3);
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


            var shipLength = 3;
            var startChange0 = new SingleChange<BimaruValue>(new GridPoint(2, 2), BimaruValue.SHIP_CONT_LEFT);
            var startChange1 = new SingleChange<BimaruValue>(new GridPoint(0, 2), BimaruValue.SHIP_CONT_LEFT);
            var startChange2 = new SingleChange<BimaruValue>(new GridPoint(1, 0), BimaruValue.SHIP_MIDDLE);
            var startChange3 = new SingleChange<BimaruValue>(new GridPoint(1, 1), BimaruValue.SHIP_MIDDLE);

            // Key is a representative SingleChange that is only present in one FieldsToChange
            var expectedChanges = new Dictionary<SingleChange<BimaruValue>, FieldsToChange<BimaruValue>>
            {
                {startChange0, new ShipLocation(startChange0.Point, Direction.LEFT, shipLength).Changes },
                {startChange1, new ShipLocation(startChange1.Point, Direction.LEFT, shipLength).Changes },
                {startChange2, new ShipLocation(new GridPoint(0, 0), Direction.UP, shipLength).Changes },
                {startChange3, new ShipLocation(new GridPoint(0, 1), Direction.UP, shipLength).Changes }
            };

            AssertEqualTrialChanges(expectedChanges, rule.GetChangeTrials(game));
        }

        [Fact]
        public void TestSingleShip()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(2, 2);
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

            AssertEqualTrialChanges(expectedChanges, rule.GetChangeTrials(game));
        }

        [Fact]
        public void TestPresetShipFields()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(3, 3);
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

            var shipLength = 2;
            var startChange0 = new SingleChange<BimaruValue>(new GridPoint(0, 0), BimaruValue.SHIP_CONT_UP);
            var startChange1 = new SingleChange<BimaruValue>(new GridPoint(2, 1), BimaruValue.SHIP_CONT_RIGHT);
            var startChange2 = new SingleChange<BimaruValue>(new GridPoint(1, 2), BimaruValue.SHIP_CONT_UP);

            // Key is a representative SingleChange that is only present in one FieldsToChange
            var expectedChanges = new Dictionary<SingleChange<BimaruValue>, FieldsToChange<BimaruValue>>
            {
                {startChange0, new ShipLocation(startChange0.Point, Direction.UP, shipLength).Changes },
                {startChange1, new ShipLocation(startChange1.Point, Direction.RIGHT, shipLength).Changes },
                {startChange2, new ShipLocation(startChange2.Point, Direction.UP, shipLength).Changes }
            };

            AssertEqualTrialChanges(expectedChanges, rule.GetChangeTrials(game));
        }

        [Fact]
        public void TestShipLength()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(3, 3);
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

            const int shipLength = 2;

            var startChange0 = new SingleChange<BimaruValue>(new GridPoint(2, 0), BimaruValue.SHIP_CONT_DOWN);

            // The following three could be avoided in a more efficient implementation
            var startChange1 = new SingleChange<BimaruValue>(new GridPoint(0, 0), BimaruValue.SHIP_CONT_UP);
            var startChange2 = new SingleChange<BimaruValue>(new GridPoint(1, 1), BimaruValue.SHIP_CONT_LEFT);
            var startChange3 = new SingleChange<BimaruValue>(new GridPoint(2, 1), BimaruValue.SHIP_CONT_LEFT);

            // Key is a representative SingleChange that is only present in one FieldsToChange
            var expectedChanges = new Dictionary<SingleChange<BimaruValue>, FieldsToChange<BimaruValue>>
            {
                {startChange0, new ShipLocation(startChange0.Point, Direction.DOWN, shipLength).Changes },
                {startChange1, new ShipLocation(startChange1.Point, Direction.UP, shipLength).Changes },
                {startChange2, new ShipLocation(startChange2.Point, Direction.LEFT, shipLength).Changes },
                {startChange3, new ShipLocation(startChange3.Point, Direction.LEFT, shipLength).Changes }
            };

            AssertEqualTrialChanges(expectedChanges, rule.GetChangeTrials(game));
        }

        [Fact]
        public void TestOnlyWaterIsMissing()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(2, 2);
            game.TargetNumberOfShipFieldsPerRow[0] = 0;
            game.TargetNumberOfShipFieldsPerRow[1] = 1;

            game.TargetNumberOfShipFieldsPerColumn[0] = 1;
            game.TargetNumberOfShipFieldsPerColumn[1] = 0;

            game.TargetNumberOfShipsPerLength[1] = 1;

            game.Grid[new GridPoint(1, 0)] = BimaruValue.SHIP_SINGLE;

            // 1xSUBMARINE
            //   10
            //   --
            // 1|S?
            // 0|??

            var rule = new LongestMissingShip();

            var actualTrials = rule.GetChangeTrials(game).ToList();

            Assert.Single(actualTrials);

            var expectedChanges = new FieldsToChange<BimaruValue>()
            {
                { new GridPoint(0, 0), BimaruValue.WATER },
                { new GridPoint(0, 1), BimaruValue.WATER },
                { new GridPoint(1, 1), BimaruValue.WATER }
            };

            AssertEqualChanges(expectedChanges, actualTrials[0]);
        }
    }
}
