using Bimaru.GameUtil;
using Bimaru.Interfaces;
using Bimaru.SolverUtil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;

namespace Bimaru.Test
{
    [TestClass]
    public class FillRowOrColumnTests
    {
        [TestMethod]
        public void TestSolverRuleWater()
        {
            var game = GetBasicGame();
            game.Grid[new GridPoint(1, 2)] = BimaruValue.SHIP_UNDETERMINED;

            new FillRowOrColumnWithWater().Solve(game);

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.WATER, BimaruValue.UNDETERMINED },
                    { BimaruValue.WATER, BimaruValue.WATER, BimaruValue.SHIP_UNDETERMINED }
                });
        }

        /// <summary>
        /// 
        ///   102
        ///   ---
        /// 1|???
        /// 2|???
        /// 
        /// </summary>
        private IGame GetBasicGame()
        {
            var game = (new GameFactory()).GenerateEmptyGame(2, 3);

            game.TargetNumberOfShipFieldsPerRow[0] = 2;
            game.TargetNumberOfShipFieldsPerRow[1] = 1;

            game.TargetNumberOfShipFieldsPerColumn[0] = 1;
            game.TargetNumberOfShipFieldsPerColumn[2] = 2;

            return game;
        }

        [TestMethod]
        public void TestSolverRuleShip()
        {
            var game = GetBasicGame();
            game.Grid[new GridPoint(0, 2)] = BimaruValue.SHIP_CONT_RIGHT;

            new FillRowOrColumnWithShips().Solve(game);

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.SHIP_CONT_RIGHT },
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.SHIP_UNDETERMINED }
                });
        }

        [TestMethod]
        public void TestFieldValueChangedRuleWater()
        {
            var game = GetBasicGame();
            using (new FieldValueChangedRuleSubscriber(game, new FillRowOrColumnWithWater()))
            {
                game.Grid[new GridPoint(0, 0)] = BimaruValue.SHIP_UNDETERMINED;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.SHIP_UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED },
                    { BimaruValue.WATER, BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED }
                });
        }

        [TestMethod]
        public void TestFieldValueChangedRuleShip()
        {
            var game = GetBasicGame();
            using (new FieldValueChangedRuleSubscriber(game, new FillRowOrColumnWithShips()))
            {
                game.Grid[new GridPoint(1, 2)] = BimaruValue.SHIP_CONT_DOWN;
            }

            game.Grid.AssertEqual(
                new[,]
                {
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.SHIP_UNDETERMINED },
                    { BimaruValue.UNDETERMINED, BimaruValue.UNDETERMINED, BimaruValue.SHIP_CONT_DOWN }
                });
        }
    }
}
