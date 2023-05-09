using Bimaru.Interface.Utility;

namespace Bimaru.Database.Entities;

public class GridValueEntity
{
    public GridValueEntity(int rowIndex, int columnIndex, BimaruValue value)
    {
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        Value = value;
    }

    public int RowIndex { get; set; }

    public int ColumnIndex { get; set; }

    public BimaruValue Value { get; set; }

    public int GameId { get; set; }

    public GameEntity Game { get; set; } = null!;
}