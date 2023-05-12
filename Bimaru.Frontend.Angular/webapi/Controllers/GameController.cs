using AutoMapper;
using Bimaru.Interface.Database;
using Bimaru.Interface.Solver;
using Microsoft.AspNetCore.Mvc;
using webapi.Mappers;
using webapi.Models;

namespace webapi.Controllers;

[ApiController]
[Route("api/games")]
[Consumes("application/json")]
[Produces("application/json")]
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

    /// <summary>
    /// Get a Bimaru game by id.
    /// </summary>
    /// <param name="id">Id of the Bimaru game</param>
    /// <returns>Bimaru game of the given id</returns>
    /// <response code="200">Returns the game</response>
    /// <response code="404">No game with that id exists</response>
    [HttpGet("{id:int}", Name = "FindGameById")]
    public async Task<ActionResult<GameWithMetaDataDto>> FindGameById(int id)
    {
        var gameEntity = await gameRepository.GetGameAsync(id);

        var game = mapper.Map<GameWithMetaDataDto>(gameEntity);

        return game != null ? Ok(game) : NotFound();
    }

    /// <summary>
    /// Get a random Bimaru game from all games filtered by size and difficulty.
    /// </summary>
    /// <param name="size">Filter on size</param>
    /// <param name="difficulty">Filter on difficulty</param>
    /// <returns>A random game from all games filtered by size and difficulty</returns>
    /// <response code="200">Returns the selected game</response>
    /// <response code="404">No game with that size and difficulty exists</response>
    [HttpGet]
    public async Task<ActionResult<GameWithMetaDataDto>> GetRandomGame(GameSize? size, GameDifficulty? difficulty)
    {
        var gameEntity = await gameRepository.GetRandomGameAsync(size, difficulty);

        var game = mapper.Map<GameWithMetaDataDto>(gameEntity);

        return game != null ? Ok(game) : NotFound();
    }

    /// <summary>
    /// Solve the Bimaru game.
    /// </summary>
    /// <param name="game">The game to be solved</param>
    /// <returns></returns>
    /// <response code="200">Returns the solved game</response>
    /// <response code="400">The game is not uniquely solvable or already solved</response>
    [HttpGet("solve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Store the Bimaru game.
    /// </summary>
    /// <param name="game">Game to store</param>
    /// <returns>The stored game with meta data</returns>
    /// <remarks>
    /// Conditions for storage:<br />
    /// - No equivalent game may already exist in the database.<br />
    /// - The game needs to be valid, uniquely solvable and not yet solved.<br />
    /// - The game's size has to be anywhere between 6x6 and 10x10.<br />
    /// </remarks>
    /// <response code="201">Returns the stored game with meta data</response>
    /// <response code="400">The game is not valid for storage</response>
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

        return CreatedAtRoute("FindGameById", new { id = createdGame.Id }, createdGame);
    }
}
