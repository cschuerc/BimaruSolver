using AutoMapper;
using Bimaru.Interface.Database;
using webapi.Mappers;
using webapi.Models;

namespace Bimaru.Tests;

public static class GameMapperFactoryForTesting
{
    public static IMapper Generate()
    {
        var config = new MapperConfiguration(cfg
            =>
        {
            cfg.AddProfile(new GameProfile());
            cfg.CreateMap<GameWithMetaDataDto, GameEntity>();
            cfg.CreateMap<GridValueDto, GridValueEntity>();
            cfg.CreateMap<GameWithMetaDataDto, GameDto>();
        });

        return config.CreateMapper();
    }
}