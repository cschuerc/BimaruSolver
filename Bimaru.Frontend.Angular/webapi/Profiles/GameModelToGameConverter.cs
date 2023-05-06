using AutoMapper;
using Bimaru.Game;
using Bimaru.Interface.Game;
using Bimaru.Interface.Utility;
using webapi.Models;

namespace webapi.Profiles;

public class GameModelToGameConverter : ITypeConverter<GameDto, IBimaruGame>
{
    public IBimaruGame Convert(GameDto source, IBimaruGame destination, ResolutionContext context)
    {
        var game = new BimaruGame(
            new GridTally(source.TargetNumberOfShipFieldsPerRow),
            new GridTally(source.TargetNumberOfShipFieldsPerColumn),
            new ShipTarget(source.TargetNumberOfShipsPerLength),
            new BimaruGrid(source.NumberOfRows, source.NumberOfColumns)
        );

        foreach (var gridValue in source.GridValues)
        {
            game.Grid[new GridPoint(gridValue.RowIndex, gridValue.ColumnIndex)] = gridValue.Value;
        }

        return game;
    }
}