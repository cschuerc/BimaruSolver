using System.Collections.Generic;
using System.Linq;
using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Bimaru.SolverUtil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;

namespace Bimaru.Test
{
    [TestClass]
    public class BruteForceTests
    {
        [TestMethod]
        public void TestFullyDetermined()
        {
            var game = (new GameFactory()).GenerateEmptyGame(1, 1);
            game.Grid[new(0, 0)] = BimaruValue.SHIP_MIDDLE;
            var rule = new BruteForce();

            Assert.AreEqual(0, rule.GetChangeTrials(game).Count());
        }

        [TestMethod]
        public void TestUndetermined()
        {
            var game = (new GameFactory()).GenerateEmptyGame(1, 1);
            var rule = new BruteForce();

            var p00 = new GridPoint(0, 0);
            AssertEqualTrialChanges(
                new List<SingleChange<BimaruValue>>()
                {
                    new(p00, BimaruValue.SHIP_CONT_DOWN),
                    new(p00, BimaruValue.SHIP_CONT_LEFT),
                    new(p00, BimaruValue.SHIP_CONT_RIGHT),
                    new(p00, BimaruValue.SHIP_CONT_UP),
                    new(p00, BimaruValue.SHIP_MIDDLE),
                    new(p00, BimaruValue.SHIP_SINGLE),
                    new(p00, BimaruValue.WATER),
                },
                rule.GetChangeTrials(game).ToList());
        }

        private static void AssertEqualTrialChanges(
            IReadOnlyCollection<SingleChange<BimaruValue>> expectedTrials,
            IReadOnlyCollection<FieldsToChange<BimaruValue>> actualTrials)
        {
            Assert.AreEqual(expectedTrials.Count, actualTrials.Count);

            foreach (var expectedChange in expectedTrials)
            {
                var actualChange = actualTrials.FirstOrDefault(a => a.Count() == 1 && a.Contains(expectedChange));
                Assert.IsNotNull(actualChange);
            }
        }

        [TestMethod]
        public void TestShipUndetermined()
        {
            var game = (new GameFactory()).GenerateEmptyGame(1, 1);
            var p00 = new GridPoint(0, 0);
            game.Grid[p00] = BimaruValue.SHIP_UNDETERMINED;

            var rule = new BruteForce();
            AssertEqualTrialChanges(
                new List<SingleChange<BimaruValue>>()
                {
                    new(p00, BimaruValue.SHIP_CONT_DOWN),
                    new(p00, BimaruValue.SHIP_CONT_LEFT),
                    new(p00, BimaruValue.SHIP_CONT_RIGHT),
                    new(p00, BimaruValue.SHIP_CONT_UP),
                    new(p00, BimaruValue.SHIP_MIDDLE),
                    new(p00, BimaruValue.SHIP_SINGLE),
                },
                rule.GetChangeTrials(game).ToList());
        }
    }
}
