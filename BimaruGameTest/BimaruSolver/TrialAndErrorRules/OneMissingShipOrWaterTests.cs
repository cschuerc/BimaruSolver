using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace Bimaru.SolverUtil
{
    [TestClass]
    public class OneMissingShipOrWaterTests
    {
        [TestMethod]
        public void TestFullyDeterminedWithoutFallback()
        {
            var game = (new GameFactory()).GenerateEmptyGame(1, 1);
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_MIDDLE;
            var rule = new OneMissingShipOrWater(null);

            Assert.ThrowsException<InvalidOperationException>(() => rule.GetChangeTrials(game).Count());
        }

        [TestMethod]
        public void TestFullyDeterminedWithFallback()
        {
            var game = (new GameFactory()).GenerateEmptyGame(1, 1);
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_MIDDLE;
            var rule = new OneMissingShipOrWater(new BruteForce());

            Assert.AreEqual(0, rule.GetChangeTrials(game).Count());
        }

        [TestMethod]
        public void TestOneMissingShip()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 4);
            game.Grid[new GridPoint(2, 3)] = BimaruValue.WATER;
            game.TargetNumberOfShipFieldsPerRow[0] = 2;
            game.TargetNumberOfShipFieldsPerRow[1] = 2;
            game.TargetNumberOfShipFieldsPerRow[2] = 1;
            game.TargetNumberOfShipFieldsPerColumn[0] = 3;
            game.TargetNumberOfShipFieldsPerColumn[1] = 0;
            game.TargetNumberOfShipFieldsPerColumn[2] = 0;
            game.TargetNumberOfShipFieldsPerColumn[3] = 2;

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

            AssertEqualTrialChanges(changesExpected, rule.GetChangeTrials(game));
        }

        private static void AssertEqualTrialChanges(
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
        public void TestOneMissingWater()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 4);
            game.Grid[new GridPoint(2, 3)] = BimaruValue.SHIP_UNDETERMINED;
            game.TargetNumberOfShipFieldsPerRow[0] = 2;
            game.TargetNumberOfShipFieldsPerRow[1] = 2;
            game.TargetNumberOfShipFieldsPerRow[2] = 3;
            game.TargetNumberOfShipFieldsPerColumn[0] = 0;
            game.TargetNumberOfShipFieldsPerColumn[1] = 3;
            game.TargetNumberOfShipFieldsPerColumn[2] = 3;
            game.TargetNumberOfShipFieldsPerColumn[3] = 1;

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

            AssertEqualTrialChanges(changesExpected, rule.GetChangeTrials(game));
        }

        [TestMethod]
        public void TestNeverOneMissing()
        {
            var game = (new GameFactory()).GenerateEmptyGame(2, 4);
            game.TargetNumberOfShipFieldsPerRow[0] = 2;
            game.TargetNumberOfShipFieldsPerRow[1] = 2;
            game.TargetNumberOfShipFieldsPerColumn[0] = 2;
            game.TargetNumberOfShipFieldsPerColumn[1] = 0;
            game.TargetNumberOfShipFieldsPerColumn[2] = 0;
            game.TargetNumberOfShipFieldsPerColumn[3] = 2;

            //   2002
            //   ----
            // 2|????
            // 2|????
            // => No row or column with one missing WATER or ship.

            var rule = new OneMissingShipOrWater(null);

            // No fall-back rule but fall-back rule is needed.
            Assert.ThrowsException<InvalidOperationException>(() => rule.GetChangeTrials(game));

            // Check fall-back rule is called
            var counter = new CountTrialCalls();
            rule = new OneMissingShipOrWater(counter);

            int numTrials = 0;
            foreach (var changes in rule.GetChangeTrials(game))
            {
                Assert.IsNull(changes);
                numTrials++;
            }

            Assert.AreEqual(1, numTrials);
            Assert.AreEqual(1, counter.NumberOfTrialCalls);
        }

        private class CountTrialCalls : ITrialAndErrorRule
        {
            public CountTrialCalls()
            {
                NumberOfTrialCalls = 0;
            }

            public int NumberOfTrialCalls
            {
                get;
                private set;
            }

            public bool AreTrialsDisjoint => true;

            public bool AreTrialsComplete => false;

            public IEnumerable<FieldsToChange<BimaruValue>> GetChangeTrials(IGame game)
            {
                NumberOfTrialCalls++;
                yield return null;
            }
        }
    }
}
