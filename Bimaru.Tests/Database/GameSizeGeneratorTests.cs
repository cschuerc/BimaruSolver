using Bimaru.Database.Entities;
using Bimaru.Interface.Database;
using Xunit;

namespace Bimaru.Tests.Database;

public class GameSizeGeneratorTests
{
    [Theory]
    [InlineData(6, 6, GameSize.SMALL)]
    [InlineData(7, 7, GameSize.SMALL)]
    [InlineData(6, 9, GameSize.MIDDLE)]
    [InlineData(8, 8, GameSize.MIDDLE)]
    [InlineData(10, 8, GameSize.MIDDLE)]
    [InlineData(9, 10, GameSize.LARGE)]
    [InlineData(10, 10, GameSize.LARGE)]
    [InlineData(11, 10, GameSize.LARGE)]
    public void TestGenerateSize(int numberOfRows, int numberOfColumns, GameSize expectedSize)
    {
        var game = new GameEntity()
        {
            NumberOfRows = numberOfRows,
            NumberOfColumns = numberOfColumns
        };
        var generator = new GameSizeGenerator();

        var actualSize = generator.GenerateValue(game);

        Assert.Equal(expectedSize, actualSize);
    }
}