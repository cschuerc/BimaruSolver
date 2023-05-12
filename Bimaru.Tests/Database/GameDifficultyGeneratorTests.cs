using Bimaru.Database.Entities;
using Bimaru.Interface.Database;
using Xunit;

namespace Bimaru.Tests.Database;

public class GameDifficultyGeneratorTests
{
    [Theory]
    [InlineData(6, 6, GameDifficulty.EASY)]
    [InlineData(7, 7, GameDifficulty.EASY)]
    [InlineData(6, 9, GameDifficulty.MIDDLE)]
    [InlineData(8, 8, GameDifficulty.MIDDLE)]
    [InlineData(10, 8, GameDifficulty.MIDDLE)]
    [InlineData(9, 10, GameDifficulty.HARD)]
    [InlineData(10, 10, GameDifficulty.HARD)]
    [InlineData(11, 10, GameDifficulty.HARD)]
    public void TestDeriveDifficulty(int numberOfRows, int numberOfColumns, GameDifficulty expectedDifficulty)
    {
        var game = new GameEntity()
        {
            NumberOfRows = numberOfRows,
            NumberOfColumns = numberOfColumns
        };

        var generator = new GameDifficultyGenerator();

        var actualDifficulty = generator.GenerateValue(game);

        Assert.Equal(expectedDifficulty, actualDifficulty);
    }
}