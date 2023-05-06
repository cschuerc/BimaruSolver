using AutoMapper;
using Bimaru.Interface.Game;
using Bimaru.Interface.Utility;
using webapi.Models;

namespace webapi.Profiles;

public class GameToGameModelConverter : ITypeConverter<IBimaruGame, GameDto>
{
    public GameDto Convert(IBimaruGame source, GameDto destination, ResolutionContext context)
    {
        destination = new GameDto
        {
            NumberOfRows = source.Grid.NumberOfRows,
            NumberOfColumns = source.Grid.NumberOfColumns,

            TargetNumberOfShipFieldsPerRow = source.TargetNumberOfShipFieldsPerRow.ToArray(),
            TargetNumberOfShipFieldsPerColumn = source.TargetNumberOfShipFieldsPerColumn.ToArray(),
            TargetNumberOfShipsPerLength = source.TargetNumberOfShipsPerLength.ToArray()
        };

        var grid = source.Grid;

        foreach (var point in grid.AllPoints().Where(p => grid[p] != BimaruValue.UNDETERMINED))
        {
            destination.GridValues.Add(new GridValueDto()
            {
                RowIndex = point.RowIndex,
                ColumnIndex = point.ColumnIndex,
                Value = grid[point]
            });
        }

        return destination;
    }
}