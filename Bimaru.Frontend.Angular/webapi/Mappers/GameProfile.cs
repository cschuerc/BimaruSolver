using AutoMapper;
using Bimaru.Interface.Database;
using webapi.Models;

namespace webapi.Mappers;

public class GameProfile : Profile
{
    public GameProfile()
    {
        CreateMap<GameEntity, GameDto>();
        CreateMap<GameEntity, GameWithMetaDataDto>();
        CreateMap<GridValueEntity, GridValueDto>();
    }
}