using System.Collections.Generic;
using System.Linq;
using Bimaru.Interface.Utility;
using Bimaru.Solver.TrialAndErrorRules;
using Xunit;

namespace Bimaru.Tests.Solver.TrialAndErrorRules
{
    public class BruteForceTests
    {
        [Fact]
        public void TestFullyDetermined()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(1, 1);
            game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_MIDDLE;
            var rule = new BruteForce();

            Assert.Empty( rule.GetChangeTrials(game));
        }

        [Fact]
        public void TestUndetermined()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(1, 1);
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
            Assert.Equal(expectedTrials.Count, actualTrials.Count);

            foreach (var expectedChange in expectedTrials)
            {
                var actualChange = actualTrials.FirstOrDefault(a => a.Count == 1 && a.Contains(expectedChange));
                Assert.NotNull(actualChange);
            }
        }

        [Fact]
        public void TestShipUndetermined()
        {
            var game = GameFactoryForTesting.GenerateEmptyGame(1, 1);
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
