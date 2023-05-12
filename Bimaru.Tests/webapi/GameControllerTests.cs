using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Bimaru.Interface.Database;
using Bimaru.Interface.Game;
using Bimaru.Interface.Utility;
using Microsoft.AspNetCore.Mvc;
using Moq;
using webapi.Controllers;
using webapi.Mappers;
using webapi.Models;
using Xunit;

namespace Bimaru.Tests.webapi;

public class GameControllerTests
{
    private static readonly IMapper mapper = GameMapperFactoryForTesting.Generate();

    private static readonly GridValueEntity expectedGridValue = new(5, 4, BimaruValue.SHIP_CONT_DOWN);
    private static readonly GameEntity expectedGame = new()
    {
        Id = 5,
        Size = GameSize.MIDDLE,
        Difficulty = GameDifficulty.HARD,
        NumberOfRows = 6,
        NumberOfColumns = 6,

        TargetNumberOfShipFieldsPerRow = new[] { 1, 2, 3 },
        TargetNumberOfShipFieldsPerColumn = new[] { 1, 2, 3 },
        TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
        GridValues = new List<GridValueEntity>()
        {
            expectedGridValue
        }
    };

    [Fact]
    public async Task TestGameByIdWhenGameNotExists()
    {
        // Arrange
        var mockRepo = new Mock<IGameRepository>();
        mockRepo.Setup(repo => repo.GetGameAsync(3))
            .ReturnsAsync(() => null);

        var controller = CreateGameController(mockRepo);

        // Act
        var result = await controller.FindGameById(3);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task TestGameByIdWhenGameExists()
    {
        // Arrange
        var mockRepo = new Mock<IGameRepository>();
        mockRepo.Setup(repo => repo.GetGameAsync(3))
            .ReturnsAsync(expectedGame);

        var controller = CreateGameController(mockRepo);

        // Act
        var result = await controller.FindGameById(3);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        expectedGame.AssertEquals(mapper.Map<GameEntity>(result.GetObjectResultContent()));
    }

    [Fact]
    public async Task TestRandomGameWhenNoGameExists()
    {
        // Arrange
        var mockRepo = new Mock<IGameRepository>();
        mockRepo.Setup(repo => repo.GetRandomGameAsync(null, null))
            .ReturnsAsync(() => null);

        var controller = CreateGameController(mockRepo);

        // Act
        var result = await controller.GetRandomGame(null, null);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task TestRandomGameWhenGameExists()
    {
        // Arrange
        var mockRepo = new Mock<IGameRepository>();
        mockRepo.Setup(repo => repo.GetRandomGameAsync(null, null))
            .ReturnsAsync(expectedGame);

        var controller = CreateGameController(mockRepo);

        // Act
        var result = await controller.GetRandomGame(null, null);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        expectedGame.AssertEquals(mapper.Map<GameEntity>(result.GetObjectResultContent()));
    }

    [Theory]
    [MemberData(nameof(GamesLeadingToBadRequest))]
    public void TestSolveGameShallLeadToBadRequest(IBimaruGame game)
    {
        // Arrange
        var gameDto = GameDtoGameMapper.ReverseMap(game);
        var controller = CreateGameController(new Mock<IGameRepository>());

        // Act
        var result = controller.SolveGame(gameDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    public static IEnumerable<object[]> GamesLeadingToBadRequest()
    {
        yield return new object[] { GameFactoryForTesting.GenerateGameNoSolution() };
        yield return new object[] { GameFactoryForTesting.GenerateGameTwoSolutions() };
        yield return new object[] { GameFactoryForTesting.GenerateInvalidGame() };
        yield return new object[] { GameFactoryForTesting.GenerateSolvedGame() };
    }


    [Fact]
    public void TestSolveUnsolvedGameWithSingleSolution()
    {
        // Arrange
        var gameDto = GameDtoGameMapper.ReverseMap(GameFactoryForTesting.GenerateGameOneSolution());
        var controller = CreateGameController(new Mock<IGameRepository>());

        // Act
        var result = controller.SolveGame(gameDto);

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
        var actualSolvedGame = result.GetObjectResultContent();
        gameDto.AssertEqualsOmitGridValues(actualSolvedGame);
        var expectedNumberGridValues = gameDto.NumberOfRows * gameDto.NumberOfColumns;
        Assert.Equal(expectedNumberGridValues, actualSolvedGame.GridValues.Count);
    }

    [Theory]
    [MemberData(nameof(GamesLeadingToBadRequest))]
    public async Task TestCreateGameShallLeadToBadRequest(IBimaruGame game)
    {
        // Arrange
        var gameDto = GameDtoGameMapper.ReverseMap(game);
        var controller = CreateGameController(new Mock<IGameRepository>());

        // Act
        var result = await controller.CreateBimaruGame(gameDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task TestCreateUnsolvedGameWithSingleSolution()
    {
        // Arrange
        var mockRepo = new Mock<IGameRepository>();
        var gameDto = GameDtoGameMapper.ReverseMap(GameFactoryForTesting.GenerateGameOneSolution());
        var controller = CreateGameController(mockRepo);

        // Act
        var result = await controller.CreateBimaruGame(gameDto);

        // Assert
        Assert.IsType<CreatedAtRouteResult>(result.Result);
        var actualCreatedGame = result.GetObjectResultContent();
        var actualCreatedGameDto = mapper.Map<GameDto>(actualCreatedGame);
        gameDto.AssertEquals(actualCreatedGameDto);
        mockRepo.Verify(m => m.AddGame(It.IsAny<GameEntity>()), Times.Once);
        mockRepo.Verify(m => m.SaveChangesAsync(), Times.Once);
    }

    private static GameController CreateGameController(IMock<IGameRepository> repoMock)
    {
        return new GameController(
            repoMock.Object,
            GameMapperFactoryForTesting.Generate(),
            SolverFactoryForTesting.GenerateSolver(true));
    }
}