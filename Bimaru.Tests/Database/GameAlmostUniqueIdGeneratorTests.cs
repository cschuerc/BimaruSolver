using System.Collections.Generic;
using System.Linq;
using Bimaru.Database.Entities;
using Bimaru.Interface.Database;
using Bimaru.Interface.Utility;
using Xunit;

namespace Bimaru.Tests.Database;

public class GameAlmostUniqueIdGeneratorTests
{
    private static GameEntity GenerateDeterministicGameEntity()
    {
        return new GameEntity()
        {
            Id = 1,
            Size = GameSize.SMALL,
            Difficulty = GameDifficulty.EASY,
            NumberOfRows = 2,
            NumberOfColumns = 3,
            TargetNumberOfShipFieldsPerRow = new[] { 2, 3 },
            TargetNumberOfShipFieldsPerColumn = new[] { 4, 5, 6 },
            TargetNumberOfShipsPerLength = new[] { 7, 8, 9, 10 },
            GridValues = new List<GridValueEntity>()
            {
                new(0, 2, BimaruValue.WATER),
                new(1, 0, BimaruValue.SHIP_MIDDLE)
            }
        };
    }

    [Theory]
    [InlineData(1, GameSize.SMALL, GameDifficulty.EASY)]
    [InlineData(2, GameSize.SMALL, GameDifficulty.EASY)]
    [InlineData(1, GameSize.LARGE, GameDifficulty.EASY)]
    [InlineData(1, GameSize.SMALL, GameDifficulty.MIDDLE)]
    public void TestEqualValueWhenSameGame(int id, GameSize size, GameDifficulty difficulty)
    {
        var game = GenerateDeterministicGameEntity();
        var gameCopy = GenerateDeterministicGameEntity();
        gameCopy.Id = id;
        gameCopy.Size = size;
        gameCopy.Difficulty = difficulty;

        var sut = new GameAlmostUniqueIdGenerator();

        Assert.Equal(sut.GenerateValue(game), sut.GenerateValue(gameCopy));
    }

    [Theory]
    [InlineData(3, 3)]
    [InlineData(2, 4)]
    public void TestDifferentValueWhenGridSizeDifferent(int numberOfRows, int numberOfColumns)
    {
        var game = GenerateDeterministicGameEntity();

        var gameCopy = GenerateDeterministicGameEntity();
        gameCopy.NumberOfRows = numberOfRows;
        gameCopy.NumberOfColumns = numberOfColumns;

        var sut = new GameAlmostUniqueIdGenerator();

        Assert.NotEqual(sut.GenerateValue(game), sut.GenerateValue(gameCopy));
    }

    [Theory]
    [InlineData(1, 3, 4, 5, 6)]
    [InlineData(2, 1, 4, 5, 6)]
    [InlineData(2, 3, 1, 5, 6)]
    [InlineData(2, 3, 4, 1, 6)]
    [InlineData(2, 3, 4, 5, 1)]
    public void TestDifferentValueWhenTargetShipFieldsDifferent(int row0, int row1, int col0, int col1, int col2)
    {
        var game = GenerateDeterministicGameEntity();

        var gameCopy = GenerateDeterministicGameEntity();
        gameCopy.TargetNumberOfShipFieldsPerRow[0] = row0;
        gameCopy.TargetNumberOfShipFieldsPerRow[1] = row1;

        gameCopy.TargetNumberOfShipFieldsPerColumn[0] = col0;
        gameCopy.TargetNumberOfShipFieldsPerColumn[1] = col1;
        gameCopy.TargetNumberOfShipFieldsPerColumn[2] = col2;

        var sut = new GameAlmostUniqueIdGenerator();

        Assert.NotEqual(sut.GenerateValue(game), sut.GenerateValue(gameCopy));
    }

    [Theory]
    [InlineData(1, 8, 9, 10)]
    [InlineData(7, 1, 9, 10)]
    [InlineData(7, 8, 1, 10)]
    [InlineData(7, 8, 9, 1)]
    
    public void TestDifferentValueWhenTargetShipsDifferent(int length1, int length2, int length3, int length4)
    {
        var game = GenerateDeterministicGameEntity();

        var gameCopy = GenerateDeterministicGameEntity();
        gameCopy.TargetNumberOfShipsPerLength[0] = length1;
        gameCopy.TargetNumberOfShipsPerLength[1] = length2;
        gameCopy.TargetNumberOfShipsPerLength[2] = length3;
        gameCopy.TargetNumberOfShipsPerLength[3] = length4;

        var sut = new GameAlmostUniqueIdGenerator();

        Assert.NotEqual(sut.GenerateValue(game), sut.GenerateValue(gameCopy));
    }

    [Theory]
    [InlineData(2, 0, BimaruValue.SHIP_MIDDLE)]
    [InlineData(1, 1, BimaruValue.SHIP_MIDDLE)]
    [InlineData(1, 0, BimaruValue.SHIP_SINGLE)]
    public void TestDifferentValueWhenGridValuesDifferent(int rowIndex0, int columnIndex0, BimaruValue value0)
    {
        var game = GenerateDeterministicGameEntity();

        var gameCopy = GenerateDeterministicGameEntity();
        gameCopy.GridValues = new List<GridValueEntity>()
        {
            gameCopy.GridValues.First(),
            new(rowIndex0, columnIndex0, value0)
        };

        var sut = new GameAlmostUniqueIdGenerator();

        Assert.NotEqual(sut.GenerateValue(game), sut.GenerateValue(gameCopy));
    }

    [Fact]
    public void TestSameValueWhenGridValuesOrderDifferent()
    {
        var game = GenerateDeterministicGameEntity();

        var gameCopy = GenerateDeterministicGameEntity();
        gameCopy.GridValues = new List<GridValueEntity>()
        {
            gameCopy.GridValues.Last(),
            gameCopy.GridValues.First()
        };

        var sut = new GameAlmostUniqueIdGenerator();

        Assert.Equal(sut.GenerateValue(game), sut.GenerateValue(gameCopy));
    }
}