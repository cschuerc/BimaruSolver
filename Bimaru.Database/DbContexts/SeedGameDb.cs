using Bimaru.Interface.Database;
using Bimaru.Interface.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Bimaru.Database.DbContexts
{
    public static class SeedGameDb
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new GameDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<GameDbContext>>());

            if (context.Games.Any())
            {
                return; // DB has been seeded
            }

            var gameEntities = GetGameEntities();

            context.GridValues.AddRange(gameEntities.SelectMany(e => e.GridValues));
            context.Games.AddRange(gameEntities);
            context.SaveChanges();
        }

        public static List<GameEntity> GetGameEntities()
        {
            return new List<GameEntity>()
            {
                new()
                {
                    Size = GameSize.SMALL,
                    Difficulty = GameDifficulty.EASY,
                    NumberOfRows = 6,
                    NumberOfColumns = 6,
                    TargetNumberOfShipFieldsPerRow = new[] { 2, 1, 3, 1, 1, 2 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 2, 1, 2, 2, 0, 3 },
                    TargetNumberOfShipsPerLength = new[] { 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(0, 1, BimaruValue.SHIP_CONT_RIGHT),
                        new(2, 2, BimaruValue.WATER),
                        new(5, 0, BimaruValue.WATER),
                        new(5, 5, BimaruValue.WATER)
                    }
                },

                new()
                {
                    Size = GameSize.SMALL,
                    Difficulty = GameDifficulty.EASY,
                    NumberOfRows = 6,
                    NumberOfColumns = 6,
                    TargetNumberOfShipFieldsPerRow = new[] { 1, 2, 2, 1, 1, 3 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 2, 2, 0, 2, 1, 3 },
                    TargetNumberOfShipsPerLength = new[] { 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(2, 4, BimaruValue.WATER),
                        new(4, 5, BimaruValue.SHIP_CONT_DOWN),
                    }
                },

                new()
                {
                    Size = GameSize.MIDDLE,
                    Difficulty = GameDifficulty.EASY,
                    NumberOfRows = 8,
                    NumberOfColumns = 8,
                    TargetNumberOfShipFieldsPerRow = new[] { 4, 0, 4, 1, 1, 4, 1, 5 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 5, 2, 2, 2, 2, 2, 2, 3 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(0, 2, BimaruValue.SHIP_MIDDLE),
                        new(2, 3, BimaruValue.SHIP_SINGLE),
                        new(2, 7, BimaruValue.SHIP_CONT_UP),
                        new(5, 0, BimaruValue.SHIP_MIDDLE),
                        new(5, 6, BimaruValue.WATER),
                    }
                },

                new()
                {
                    Size = GameSize.MIDDLE,
                    Difficulty = GameDifficulty.EASY,
                    NumberOfRows = 8,
                    NumberOfColumns = 8,
                    TargetNumberOfShipFieldsPerRow = new[] { 4, 2, 0, 3, 2, 3, 2, 4 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 5, 0, 3, 4, 3, 1, 1, 3 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(5, 0, BimaruValue.WATER),
                        new(6, 6, BimaruValue.SHIP_SINGLE),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.EASY,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 1, 2, 1, 2, 3, 2, 2, 3, 1, 3 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 2, 5, 2, 1, 0, 2, 1, 0, 4, 3 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(0, 0, BimaruValue.SHIP_SINGLE),
                        new(1, 9, BimaruValue.WATER),
                        new(2, 8, BimaruValue.SHIP_CONT_UP),
                        new(7, 1, BimaruValue.SHIP_CONT_RIGHT),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.EASY,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 1, 3, 2, 2, 3, 1, 1, 1, 5, 1 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 2, 3, 2, 0, 1, 2, 5, 0, 2, 3 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(1, 6, BimaruValue.WATER),
                        new(2, 2, BimaruValue.SHIP_CONT_DOWN),
                        new(7, 9, BimaruValue.SHIP_MIDDLE),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.EASY,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 3, 1, 3, 0, 3, 2, 4, 1, 2, 1 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 2, 0, 4, 1, 2, 2, 2, 4, 1, 2 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(0, 7, BimaruValue.SHIP_MIDDLE),
                        new(2, 4, BimaruValue.SHIP_CONT_LEFT),
                        new(4, 2, BimaruValue.WATER),
                        new(4, 7, BimaruValue.WATER),
                        new(7, 2, BimaruValue.SHIP_CONT_UP),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.EASY,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 1, 1, 2, 2, 1, 3, 1, 2, 5, 2 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 2, 3, 1, 0, 1, 5, 0, 5, 2, 1 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(1, 8, BimaruValue.SHIP_CONT_UP),
                        new(2, 1, BimaruValue.SHIP_CONT_UP),
                        new(8, 7, BimaruValue.WATER),
                    }
                },

                new()
                {
                    Size = GameSize.MIDDLE,
                    Difficulty = GameDifficulty.MIDDLE,
                    NumberOfRows = 8,
                    NumberOfColumns = 8,
                    TargetNumberOfShipFieldsPerRow = new[] { 4, 1, 3, 2, 2, 3, 1, 4 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 6, 1, 3, 2, 3, 0, 2, 3 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(0, 1, BimaruValue.SHIP_CONT_LEFT),
                        new(1, 6, BimaruValue.SHIP_CONT_UP),
                        new(2, 2, BimaruValue.WATER),
                        new(4, 6, BimaruValue.WATER),
                    }
                },

                new()
                {
                    Size = GameSize.MIDDLE,
                    Difficulty = GameDifficulty.MIDDLE,
                    NumberOfRows = 8,
                    NumberOfColumns = 8,
                    TargetNumberOfShipFieldsPerRow = new[] { 2, 1, 4, 3, 2, 4, 1, 3 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 4, 2, 1, 4, 1, 5, 1, 2 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(1, 1, BimaruValue.WATER),
                        new(1, 4, BimaruValue.WATER),
                        new(2, 0, BimaruValue.SHIP_CONT_RIGHT),
                        new(5, 0, BimaruValue.SHIP_CONT_UP),
                        new(5, 7, BimaruValue.WATER),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.MIDDLE,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 2, 1, 2, 1, 3, 3, 1, 3, 1, 3 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 3, 2, 3, 1, 0, 4, 1, 2, 1, 3 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(1, 9, BimaruValue.SHIP_CONT_UP),
                        new(4, 0, BimaruValue.SHIP_CONT_UP),
                        new(4, 7, BimaruValue.WATER),
                        new(5, 9, BimaruValue.WATER),
                        new(8, 2, BimaruValue.SHIP_CONT_DOWN),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.MIDDLE,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 2, 2, 2, 1, 3, 3, 1, 4, 1, 1 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 2, 2, 2, 3, 2, 3, 1, 1, 1, 3 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(0, 5, BimaruValue.SHIP_CONT_LEFT),
                        new(1, 9, BimaruValue.WATER),
                        new(2, 9, BimaruValue.SHIP_SINGLE),
                        new(5, 7, BimaruValue.SHIP_MIDDLE),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.MIDDLE,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 2, 4, 0, 2, 4, 1, 1, 2, 2, 2 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 1, 4, 1, 1, 3, 2, 3, 1, 3, 1 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(0, 6, BimaruValue.SHIP_CONT_LEFT),
                        new(3, 6, BimaruValue.WATER),
                        new(5, 2, BimaruValue.WATER),
                        new(7, 1, BimaruValue.SHIP_SINGLE),
                        new(7, 8, BimaruValue.SHIP_MIDDLE),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.MIDDLE,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 1, 1, 3, 1, 2, 5, 1, 2, 1, 3 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 1, 5, 1, 2, 2, 3, 1, 1, 2, 2 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(0, 1, BimaruValue.SHIP_SINGLE),
                        new(2, 6, BimaruValue.SHIP_MIDDLE),
                        new(8, 9, BimaruValue.SHIP_CONT_UP),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.MIDDLE,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 4, 3, 1, 1, 1, 1, 3, 2, 3, 1 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 1, 1, 1, 5, 1, 3, 2, 3, 1, 2 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(0, 2, BimaruValue.SHIP_CONT_RIGHT),
                        new(1, 0, BimaruValue.WATER),
                        new(4, 7, BimaruValue.SHIP_SINGLE),
                        new(7, 9, BimaruValue.SHIP_CONT_UP),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.MIDDLE,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 3, 2, 1, 4, 2, 2, 1, 2, 1, 2 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 1, 4, 2, 2, 1, 2, 1, 5, 1, 1 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(3, 6, BimaruValue.SHIP_MIDDLE),
                        new(7, 2, BimaruValue.SHIP_CONT_DOWN),
                        new(7, 9, BimaruValue.SHIP_SINGLE),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.HARD,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 1, 4, 1, 2, 2, 1, 5, 1, 2, 1 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 1, 1, 1, 2, 3, 4, 1, 1, 4, 2 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(0, 1, BimaruValue.SHIP_SINGLE),
                        new(1, 7, BimaruValue.SHIP_CONT_LEFT),
                        new(7, 0, BimaruValue.SHIP_SINGLE),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.HARD,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 1, 4, 1, 1, 6, 1, 2, 1, 1, 2 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 1, 2, 3, 1, 2, 2, 4, 1, 3, 1 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(1, 2, BimaruValue.SHIP_CONT_UP),
                        new(1, 4, BimaruValue.WATER),
                        new(2, 7, BimaruValue.WATER),
                        new(7, 1, BimaruValue.SHIP_SINGLE),
                        new(9, 5, BimaruValue.SHIP_CONT_RIGHT),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.HARD,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 2, 1, 1, 5, 2, 1, 3, 1, 2, 2 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 2, 2, 3, 1, 1, 1, 2, 4, 2, 2 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(0, 5, BimaruValue.SHIP_SINGLE),
                        new(5, 7, BimaruValue.WATER),
                        new(6, 1, BimaruValue.SHIP_CONT_RIGHT),
                        new(8, 6, BimaruValue.SHIP_CONT_DOWN),
                        new(9, 2, BimaruValue.WATER),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.HARD,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 2, 2, 5, 1, 3, 1, 3, 1, 1, 1 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 4, 1, 2, 3, 1, 3, 1, 1, 1, 3 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(1, 8, BimaruValue.SHIP_SINGLE),
                        new(5, 5, BimaruValue.SHIP_CONT_DOWN),
                        new(6, 0, BimaruValue.SHIP_SINGLE),
                        new(7, 3, BimaruValue.WATER),
                        new(8, 5, BimaruValue.WATER),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.HARD,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 3, 1, 3, 2, 2, 1, 1, 1, 5, 1 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 1, 2, 3, 3, 1, 2, 3, 2, 1, 2 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(1, 5, BimaruValue.SHIP_SINGLE),
                        new(3, 0, BimaruValue.SHIP_SINGLE),
                        new(4, 9, BimaruValue.SHIP_CONT_UP),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.HARD,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 4, 1, 1, 3, 1, 3, 1, 1, 3, 2 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 1, 1, 3, 4, 2, 1, 1, 4, 1, 2 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(3, 2, BimaruValue.WATER),
                        new(5, 9, BimaruValue.SHIP_CONT_UP),
                        new(7, 4, BimaruValue.WATER),
                        new(8, 0, BimaruValue.SHIP_SINGLE),
                        new(8, 3, BimaruValue.SHIP_CONT_UP),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.HARD,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 3, 1, 1, 5, 1, 2, 1, 3, 2, 1 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 1, 4, 1, 1, 1, 1, 3, 1, 3, 4 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(0, 5, BimaruValue.SHIP_SINGLE),
                        new(3, 2, BimaruValue.SHIP_SINGLE),
                        new(4, 9, BimaruValue.SHIP_MIDDLE),
                        new(6, 0, BimaruValue.WATER),
                        new(9, 9, BimaruValue.WATER),
                    }
                },

                new()
                {
                    Size = GameSize.LARGE,
                    Difficulty = GameDifficulty.HARD,
                    NumberOfRows = 10,
                    NumberOfColumns = 10,
                    TargetNumberOfShipFieldsPerRow = new[] { 2, 1, 4, 2, 1, 2, 2, 1, 4, 1 },
                    TargetNumberOfShipFieldsPerColumn = new[] { 3, 2, 2, 2, 2, 1, 1, 3, 3, 1 },
                    TargetNumberOfShipsPerLength = new[] { 4, 3, 2, 1 },
                    GridValues = new List<GridValueEntity>()
                    {
                        new(0, 3, BimaruValue.SHIP_CONT_RIGHT),
                        new(0, 6, BimaruValue.WATER),
                        new(3, 6, BimaruValue.WATER),
                        new(7, 0, BimaruValue.SHIP_CONT_DOWN),
                        new(8, 9, BimaruValue.SHIP_SINGLE),
                    }
                }
            };
        }
    }
}
