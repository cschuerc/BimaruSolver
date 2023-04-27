using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Bimaru.SolverUtil;
using Xunit;
using Utility;

namespace Bimaru.Test
{
    public class SetShipEnvironmentTests
    {
        [Fact]
        public void TestUndetermined()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (new FieldValueChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.UNDETERMINED;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED }
                });
        }

        [Fact]
        public void TestWater()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (new FieldValueChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.WATER;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.WATER, BimaruValue.UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED }
                });
        }

        [Fact]
        public void TestSingleShip()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (new FieldValueChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_SINGLE;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER },
                    { BimaruValue.WATER, BimaruValue.SHIP_SINGLE, BimaruValue.WATER },
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER }
                });
        }

        [Fact]
        public void TestMiddleShip()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (new FieldValueChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_MIDDLE;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.WATER, BimaruValue.UNDETERMINED, BimaruValue.WATER },
                    { BimaruValue.UNDETERMINED, BimaruValue.SHIP_MIDDLE, BimaruValue.UNDETERMINED },
                    { BimaruValue.WATER, BimaruValue.UNDETERMINED, BimaruValue.WATER }
                });
        }

        [Fact]
        public void TestShipUndetermined()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (new FieldValueChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_UNDETERMINED;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.WATER, BimaruValue.UNDETERMINED, BimaruValue.WATER },
                    { BimaruValue.UNDETERMINED, BimaruValue.SHIP_UNDETERMINED, BimaruValue.UNDETERMINED },
                    { BimaruValue.WATER, BimaruValue.UNDETERMINED, BimaruValue.WATER }
                });
        }

        [Fact]
        public void TestShipContRight()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (new FieldValueChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_CONT_RIGHT;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER },
                    { BimaruValue.WATER, BimaruValue.SHIP_CONT_RIGHT, BimaruValue.SHIP_UNDETERMINED },
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER }
                });
        }

        [Fact]
        public void TestShipContLeft()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (new FieldValueChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_CONT_LEFT;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER },
                    { BimaruValue.SHIP_UNDETERMINED, BimaruValue.SHIP_CONT_LEFT, BimaruValue.WATER },
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER }
                });
        }

        [Fact]
        public void TestShipContUp()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (new FieldValueChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_CONT_UP;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER },
                    { BimaruValue.WATER, BimaruValue.SHIP_CONT_UP, BimaruValue.WATER },
                    { BimaruValue.WATER, BimaruValue.SHIP_UNDETERMINED, BimaruValue.WATER }
                });
        }

        [Fact]
        public void TestShipContDown()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (new FieldValueChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_CONT_DOWN;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.WATER, BimaruValue.SHIP_UNDETERMINED, BimaruValue.WATER },
                    { BimaruValue.WATER, BimaruValue.SHIP_CONT_DOWN, BimaruValue.WATER },
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER }
                });
        }

        [Fact]
        public void TestShipContOverwrite()
        {
            var game = (new GameFactory()).GenerateEmptyGame(3, 3);
            using (new FieldValueChangedRuleSubscriber(game, new SetShipEnvironment()))
            {
                game.Grid[new GridPoint(1, 1)] = BimaruValue.SHIP_CONT_RIGHT;
                game.Grid[new GridPoint(1, 2)] = BimaruValue.SHIP_CONT_LEFT;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER },
                    { BimaruValue.WATER, BimaruValue.SHIP_CONT_RIGHT, BimaruValue.SHIP_CONT_LEFT },
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.WATER }
                });
        }
    }
}
