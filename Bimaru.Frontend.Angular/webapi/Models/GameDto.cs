using System.ComponentModel.DataAnnotations;

namespace webapi.Models;

/// <summary>
/// Bimaru game without meta data
/// </summary>
public class GameDto
{
    /// <summary>
    /// Number of rows
    /// </summary>
    [Range(6, 10)]
    public int NumberOfRows { get; set; }

    /// <summary>
    /// Number of columns
    /// </summary>
    [Range(6, 10)]
    public int NumberOfColumns { get; set; }

    /// <summary>
    /// Target number of ship fields per row
    /// </summary>
    [Required]
    public int[] TargetNumberOfShipFieldsPerRow { get; set; } = null!;

    /// <summary>
    /// Target number of ship fields per column
    /// </summary>
    [Required]
    public int[] TargetNumberOfShipFieldsPerColumn { get; set; } = null!;

    /// <summary>
    /// Target number of ships of a given length (e.g. [4,2,1] means 4 ships of length 1, 2 of length 2 and 1 of length 3)
    /// </summary>
    [Required]
    public int[] TargetNumberOfShipsPerLength { get; set; } = null!;

    /// <summary>
    /// Collection of set values (e.g. water). Not set fields can be omitted.
    /// </summary>
    [Required]
    public ICollection<GridValueDto> GridValues { get; set; } = new List<GridValueDto>();
}