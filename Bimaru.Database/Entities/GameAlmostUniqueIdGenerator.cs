using Bimaru.Interface.Database;

namespace Bimaru.Database.Entities;

public class GameAlmostUniqueIdGenerator : IDerivedValueGenerator<GameEntity, int>
{
    public int GenerateValue(GameEntity entity)
    {
        var almostUniqueId = 0;

        almostUniqueId = Combine(almostUniqueId, entity.NumberOfRows);
        almostUniqueId = Combine(almostUniqueId, entity.NumberOfColumns);
        almostUniqueId = entity.TargetNumberOfShipFieldsPerRow.Aggregate(almostUniqueId, Combine);
        almostUniqueId = entity.TargetNumberOfShipFieldsPerColumn.Aggregate(almostUniqueId, Combine);
        almostUniqueId = entity.TargetNumberOfShipsPerLength
            .Reverse()
            .SkipWhile(t => t == 0) // Trailing 0s lead to equivalent games.
            .Aggregate(almostUniqueId, Combine);

        foreach (var gridValue in entity.GridValues.OrderBy(v => v.RowIndex).ThenBy(v => v.ColumnIndex))
        {
            almostUniqueId = Combine(almostUniqueId, gridValue.RowIndex);
            almostUniqueId = Combine(almostUniqueId, gridValue.ColumnIndex);
            almostUniqueId = Combine(almostUniqueId, (int)gridValue.Value);
        }

        return almostUniqueId;
    }

    private static int Combine(int value1, int value2)
    {
        return value1 * 31 + value2;
    }
}