using Bimaru.Interface.Utility;

namespace webapi.Models;

public class GridValueDto
{
    public int RowIndex { get; set; }

    public int ColumnIndex { get; set; }

    public BimaruValue Value { get; set; }
}