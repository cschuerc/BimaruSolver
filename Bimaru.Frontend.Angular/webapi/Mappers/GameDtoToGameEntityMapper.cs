using Bimaru.Interface.Database;
using webapi.Models;

namespace webapi.Mappers;

public static class GameDtoToGameEntityMapper
{
    public static GameEntity MapGame(GameDto gameDto)
    {
        return new GameEntity()
        {
            Size = DeriveSize(gameDto),
            Difficulty = DeriveDifficulty(gameDto),
            NumberOfRows = gameDto.NumberOfRows,
            NumberOfColumns = gameDto.NumberOfColumns,
            TargetNumberOfShipFieldsPerRow = (int[])gameDto.TargetNumberOfShipFieldsPerRow.Clone(),
            TargetNumberOfShipFieldsPerColumn = (int[])gameDto.TargetNumberOfShipFieldsPerColumn.Clone(),
            TargetNumberOfShipsPerLength = (int[])gameDto.TargetNumberOfShipsPerLength.Clone(),
            GridValues = gameDto.GridValues.Select(MapGridValue).ToList()
        };
    }

    private static GameSize DeriveSize(GameDto gameDto)
    {
        return (gameDto.NumberOfRows * gameDto.NumberOfColumns) switch
        {
            >= 82 => GameSize.LARGE,
            <= 50 => GameSize.SMALL,
            _ => GameSize.MIDDLE
        };
    }

    private static GameDifficulty DeriveDifficulty(GameDto gameDto)
    {
        return DeriveSize(gameDto) switch
        {
            GameSize.SMALL => GameDifficulty.EASY,
            GameSize.MIDDLE => GameDifficulty.MIDDLE,
            _ => GameDifficulty.HARD
        };
    }

    public static GridValueEntity MapGridValue(GridValueDto gridValueDto)
    {
        return new GridValueEntity(gridValueDto.RowIndex, gridValueDto.ColumnIndex, gridValueDto.Value);
    }
}