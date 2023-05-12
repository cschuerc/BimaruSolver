using Bimaru.Interface.Database;

namespace Bimaru.Database.Entities;

public class GameDifficultyGenerator : IDerivedValueGenerator<GameEntity, GameDifficulty>
{
    public GameDifficulty GenerateValue(GameEntity entity)
    {
        return (entity.NumberOfRows * entity.NumberOfColumns) switch
        {
            >= 82 => GameDifficulty.HARD,
            <= 50 => GameDifficulty.EASY,
            _ => GameDifficulty.MIDDLE
        };
    }
}