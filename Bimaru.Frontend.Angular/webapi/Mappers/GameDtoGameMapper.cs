using Bimaru.Game;
using Bimaru.Interface.Game;
using Bimaru.Interface.Utility;
using webapi.Models;

namespace webapi.Mappers;

public static class GameDtoGameMapper
{
    public static IBimaruGame Map(GameDto gameDto)
    {
        var game = new BimaruGame(
            new GridTally(gameDto.TargetNumberOfShipFieldsPerRow),
            new GridTally(gameDto.TargetNumberOfShipFieldsPerColumn),
            new ShipTarget(gameDto.TargetNumberOfShipsPerLength),
            new BimaruGrid(gameDto.NumberOfRows, gameDto.NumberOfColumns)
        );

        foreach (var gridValue in gameDto.GridValues)
        {
            game.Grid[new GridPoint(gridValue.RowIndex, gridValue.ColumnIndex)] = gridValue.Value;
        }

        return game;
    }

    public static GameDto ReverseMap(IBimaruGame game)
    {
        var gameDto = new GameDto
        {
            NumberOfRows = game.Grid.NumberOfRows,
            NumberOfColumns = game.Grid.NumberOfColumns,

            TargetNumberOfShipFieldsPerRow = game.TargetNumberOfShipFieldsPerRow.ToArray(),
            TargetNumberOfShipFieldsPerColumn = game.TargetNumberOfShipFieldsPerColumn.ToArray(),
            TargetNumberOfShipsPerLength = game.TargetNumberOfShipsPerLength.ToArray()
        };

        var grid = game.Grid;

        foreach (var point in grid.AllPoints().Where(p => grid[p] != BimaruValue.UNDETERMINED))
        {
            gameDto.GridValues.Add(new GridValueDto()
            {
                RowIndex = point.RowIndex,
                ColumnIndex = point.ColumnIndex,
                Value = grid[point]
            });
        }

        return gameDto;
    }
}