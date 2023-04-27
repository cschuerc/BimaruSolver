using System.Linq;
using Bimaru.Game;
using Bimaru.Interface.Utility;
using Xunit;

namespace Bimaru.Tests.Game
{
    public class GameFactoryTests
    {
        [Fact]
        public void TestGenerateEmptyGameRowTally()
        {
            var game = new GameFactory().GenerateEmptyGame(3, 4);

            Assert.True(game.TargetNumberOfShipFieldsPerRow.SequenceEqual(new[] { 0, 0, 0 }));
        }

        [Fact]
        public void TestGenerateEmptyGameColumnTally()
        {
            var game = new GameFactory().GenerateEmptyGame(3, 4);

            Assert.True(game.TargetNumberOfShipFieldsPerColumn.SequenceEqual(new[] { 0, 0, 0, 0 }));
        }

        [Fact]
        public void TestGenerateEmptyGameShipTarget()
        {
            var game = new GameFactory().GenerateEmptyGame(3, 4);

            Assert.Null(game.TargetNumberOfShipsPerLength.LongestShipLength);
        }

        [Fact]
        public void TestGenerateEmptyGameGrid()
        {
            var game = new GameFactory().GenerateEmptyGame(3, 4);

            foreach (var p in game.Grid.AllPoints())
            {
                Assert.Equal(BimaruValue.UNDETERMINED, game.Grid[p]);
            }
        }
    }
}
