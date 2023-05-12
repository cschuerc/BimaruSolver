using AutoMapper;
using Bimaru.Interface.Database;
using webapi.Models;

namespace webapi.Mappers;

public class GameProfile : Profile
{
    public GameProfile()
    {
        // For reading
        CreateMap<GameEntity, GameDto>();
        CreateMap<GameEntity, GameWithMetaDataDto>();
        CreateMap<GridValueEntity, GridValueDto>();

        // For persisting
        CreateMap<GameDto, GameEntity>();
        CreateMap<GridValueDto, GridValueEntity>();
    }
}