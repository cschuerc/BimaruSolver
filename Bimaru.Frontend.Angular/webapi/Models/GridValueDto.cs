using Bimaru.Interface.Utility;

namespace webapi.Models;

/// <summary>
/// Single set grid value
/// </summary>
public class GridValueDto
{
    /// <summary>
    /// Zero-based row index of the value
    /// </summary>
    public int RowIndex { get; set; }

    /// <summary>
    /// Zero-based column index of the value
    /// </summary>
    public int ColumnIndex { get; set; }

    /// <summary>
    /// Value itself (e.g. water)
    /// </summary>
    public BimaruValue Value { get; set; }
}