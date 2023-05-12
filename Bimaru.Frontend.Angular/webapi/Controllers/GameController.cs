using AutoMapper;
using Bimaru.Interface.Database;
using Bimaru.Interface.Solver;
using Microsoft.AspNetCore.Mvc;
using webapi.Mappers;
using webapi.Models;

namespace webapi.Controllers;

[ApiController]
[Route("api/games")]
public class GameController : ControllerBase
{
    private readonly IGameRepository gameRepository;
    private readonly IMapper mapper;
    private readonly IBimaruSolver solver;

    public GameController(
        IGameRepository gameRepository,
        IMapper mapper,
        IBimaruSolver solver)
    {
        this.gameRepository = gameRepository;
        this.mapper = mapper;
        this.solver = solver;
    }

    [HttpGet("{id:int}", Name = "GetGameById")]
    public async Task<ActionResult<GameWithMetaDataDto>> GetGameById(int id)
    {
        var gameEntity = await gameRepository.GetGameAsync(id);

        var game = mapper.Map<GameWithMetaDataDto>(gameEntity);

        return game != null ? Ok(game) : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<GameWithMetaDataDto>> GetRandomGame(GameSize? size, GameDifficulty? difficulty)
    {
        var gameEntity = await gameRepository.GetRandomGameAsync(size, difficulty);

        var game = mapper.Map<GameWithMetaDataDto>(gameEntity);

        return game != null ? Ok(game) : NotFound();
    }

    [HttpGet("solve")]
    public ActionResult<GameDto> SolveGame(GameDto game)
    {
        var bimaruGame = GameDtoGameMapper.Map(game);

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
            1 => Ok(GameDtoGameMapper.ReverseMap(bimaruGame)),
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

        return CreatedAtRoute("GetGameById", new { id = createdGame.Id }, createdGame);
    }
}
