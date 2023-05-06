using AutoMapper;
using Bimaru.Interface.Game;
using webapi.Entities;
using webapi.Models;

namespace webapi.Profiles;

public class GameProfile : Profile
{
    public GameProfile()
    {
        // For getting
        CreateMap<GameEntity, GameWithMetaDataDto>();
        CreateMap<GridValueEntity, GridValueDto>();

        // For solving
        CreateMap<GameDto, IBimaruGame>().ConvertUsing(new GameModelToGameConverter());
        CreateMap<IBimaruGame, GameDto>().ConvertUsing(new GameToGameModelConverter());

        // For persisting
        CreateMap<GameDto, GameEntity>();
        CreateMap<GridValueDto, GridValueEntity>();
    }
}