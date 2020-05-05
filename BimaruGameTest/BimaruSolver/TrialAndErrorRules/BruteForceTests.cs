using BimaruGame;
using BimaruInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace BimaruSolver
{
    [TestClass]
    public class BruteForceTests
    {
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
            var game = (new GameFactory()).GenerateEmptyGame(1, 1);
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_MIDDLE;
            var rule = new BruteForce();
            Assert.AreEqual(0, rule.GetCompleteChangeTrials(game).Count());
        }

        [TestMethod]
        public void TestUndetermined()
        {
            var game = (new GameFactory()).GenerateEmptyGame(1, 1);

            var p00 = new GridPoint(0, 0);
            var changesExpected = new List<SingleChange<BimaruValue>>()
            {
                new SingleChange<BimaruValue>(p00, BimaruValue.SHIP_CONT_DOWN),
                new SingleChange<BimaruValue>(p00, BimaruValue.SHIP_CONT_LEFT),
                new SingleChange<BimaruValue>(p00, BimaruValue.SHIP_CONT_RIGHT),
                new SingleChange<BimaruValue>(p00, BimaruValue.SHIP_CONT_UP),
                new SingleChange<BimaruValue>(p00, BimaruValue.SHIP_MIDDLE),
                new SingleChange<BimaruValue>(p00, BimaruValue.SHIP_SINGLE),
                new SingleChange<BimaruValue>(p00, BimaruValue.WATER),
            };

            var rule = new BruteForce();
            CheckCorrectTrialChanges(changesExpected, rule.GetCompleteChangeTrials(game));
        }

        [TestMethod]
        public void TestShipUndetermined()
        {
            var game = (new GameFactory()).GenerateEmptyGame(1, 1);

            var p00 = new GridPoint(0, 0);
            game.Grid[p00] = BimaruValue.SHIP_UNDETERMINED;

            var changesExpected = new List<SingleChange<BimaruValue>>()
            {
                new SingleChange<BimaruValue>(p00, BimaruValue.SHIP_CONT_DOWN),
                new SingleChange<BimaruValue>(p00, BimaruValue.SHIP_CONT_LEFT),
                new SingleChange<BimaruValue>(p00, BimaruValue.SHIP_CONT_RIGHT),
                new SingleChange<BimaruValue>(p00, BimaruValue.SHIP_CONT_UP),
                new SingleChange<BimaruValue>(p00, BimaruValue.SHIP_MIDDLE),
                new SingleChange<BimaruValue>(p00, BimaruValue.SHIP_SINGLE),
            };

            var rule = new BruteForce();
            CheckCorrectTrialChanges(changesExpected, rule.GetCompleteChangeTrials(game));
        }
    }
}
