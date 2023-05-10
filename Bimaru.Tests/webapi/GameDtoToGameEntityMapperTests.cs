using System;
using System.Collections.Generic;
using AutoMapper;
using Bimaru.Interface.Database;
using Bimaru.Interface.Utility;
using webapi.Mappers;
using webapi.Models;
using Xunit;

namespace Bimaru.Tests.webapi;

public class GameDtoToGameEntityMapperTests
{
    private static readonly IMapper mapper = GameMapperFactoryForTesting.Generate();

    [Fact]
    public void TestMappingExceptSizeAndDifficulty()
    {
        var game = new GameDto()
        {
            NumberOfRows = 6,
            NumberOfColumns = 7,
            TargetNumberOfShipFieldsPerRow = new[]{1,4},
            TargetNumberOfShipFieldsPerColumn = new[] { 6, 2, 5 },
            TargetNumberOfShipsPerLength = new[] { 3 },
            GridValues = new List<GridValueDto>()
            {
                new()
                {
                    RowIndex = 3,
                    ColumnIndex = 4,
                    Value = BimaruValue.SHIP_CONT_DOWN
                }
            }
        };

        var actualEntity = GameDtoToGameEntityMapper.MapGame(game);

        game.AssertEquals(mapper.Map<GameDto>(actualEntity));
    }

    [Theory]
    [InlineData(6, 6, GameSize.SMALL)]
    [InlineData(7, 7, GameSize.SMALL)]
    [InlineData(6, 9, GameSize.MIDDLE)]
    [InlineData(8, 8, GameSize.MIDDLE)]
    [InlineData(10, 8, GameSize.MIDDLE)]
    [InlineData(9, 10, GameSize.LARGE)]
    [InlineData(10, 10, GameSize.LARGE)]
    [InlineData(11, 10, GameSize.LARGE)]
    public void TestDeriveSize(int numberOfRows, int numberOfColumns, GameSize expectedSize)
    {
        var game = new GameDto()
        {
            NumberOfRows = numberOfRows,
            NumberOfColumns = numberOfColumns,
            TargetNumberOfShipFieldsPerRow = Array.Empty<int>(),
            TargetNumberOfShipFieldsPerColumn = Array.Empty<int>(),
            TargetNumberOfShipsPerLength = Array.Empty<int>()
        };

        var actualSize = GameDtoToGameEntityMapper.MapGame(game).Size;

        Assert.Equal(expectedSize, actualSize);
    }

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
        var game = new GameDto()
        {
            NumberOfRows = numberOfRows,
            NumberOfColumns = numberOfColumns,
            TargetNumberOfShipFieldsPerRow = Array.Empty<int>(),
            TargetNumberOfShipFieldsPerColumn = Array.Empty<int>(),
            TargetNumberOfShipsPerLength = Array.Empty<int>()
        };

        var actualDifficulty = GameDtoToGameEntityMapper.MapGame(game).Difficulty;

        Assert.Equal(expectedDifficulty, actualDifficulty);
    }
}