using AutoMapper;
using Bimaru.Interface.Game;
using Bimaru.Interface.Solver;
using Microsoft.AspNetCore.Mvc;
using webapi.Entities;
using webapi.Models;
using webapi.Services;

namespace webapi.Controllers;

[ApiController]
[Route("api/games")]
public class BimaruDatabaseController : ControllerBase
{
    private readonly IGameRepository gameRepository;
    private readonly IMapper mapper;
    private readonly IBimaruSolver solver;

    public BimaruDatabaseController(IGameRepository gameRepository, IMapper mapper, IBimaruSolver solver)
    {
        this.gameRepository = gameRepository;
        this.mapper = mapper;
        this.solver = solver;
    }

    [HttpGet]
    public async Task<ActionResult<GameWithMetaDataDto>> GetRandomGame()
    {
        var gameEntity = await gameRepository.GetRandomGameAsync();

        var game = mapper.Map<GameWithMetaDataDto>(gameEntity);

        return game == null ? NotFound() : Ok(game);
    }

    [HttpGet("{id:int}", Name = "Game")]
    public async Task<ActionResult<GameWithMetaDataDto>> GetGameById(int id)
    {
        var gameEntity = await gameRepository.GetGameAsync(id);

        var game = mapper.Map<GameWithMetaDataDto>(gameEntity);

        return game == null ? NotFound() : Ok(game);
    }

    [HttpGet("solve")]
    public ActionResult<GameDto> SolveGame(GameDto game)
    {
        var bimaruGame = mapper.Map<IBimaruGame>(game);

        if (!bimaruGame.IsValid)
        {
            return BadRequest("Invalid Bimaru game.");
        }

        if (bimaruGame.IsSolved)
        {
            return BadRequest("Already solved Bimaru game.");
        }

        var numberOfSolutions = solver.Solve(bimaruGame);
        return numberOfSolutions switch
        {
            1 => Ok(mapper.Map<GameDto>(bimaruGame)),
            0 => BadRequest("Bimaru game with no solution."),
            _ => BadRequest("Bimaru game with multiple solutions.")
        };
    }

    [HttpPost]
    public async Task<ActionResult<GameWithMetaDataDto>> CreateBimaruGame(GameDto game)
    {
        var actionResult = SolveGame(game);

        if (actionResult.Result is not OkObjectResult)
        {
            return BadRequest(actionResult.Result);
        }

        var gameEntity = mapper.Map<GameEntity>(game);
        gameRepository.AddGame(gameEntity);
        await gameRepository.SaveChangesAsync();

        var createdGame = mapper.Map<GameWithMetaDataDto>(gameEntity);

        return CreatedAtRoute("Game", new { id = createdGame.Id }, createdGame);
    }
}
