using Bimaru.Interface.Database;

namespace Bimaru.Database.Entities;

public class GameSizeGenerator : IDerivedValueGenerator<GameEntity, GameSize>
{
    public GameSize GenerateValue(GameEntity entity)
    {
        return (entity.NumberOfRows * entity.NumberOfColumns) switch
        {
            >= 82 => GameSize.LARGE,
            <= 50 => GameSize.SMALL,
            _ => GameSize.MIDDLE
        };
    }
}