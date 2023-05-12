using Bimaru.Interface.Database;

namespace webapi.Models;

/// <summary>
/// Bimaru game with meta data
/// </summary>
public class GameWithMetaDataDto
{
    /// <summary>
    /// Meta data: Game identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Meta data: Game size
    /// </summary>
    public GameSize Size { get; set; }

    /// <summary>
    /// Meta data: Game difficulty
    /// </summary>
    public GameDifficulty Difficulty { get; set; }

    /// <summary>
    /// Number of rows
    /// </summary>
    public int NumberOfRows { get; set; }

    /// <summary>
    /// Number of columns
    /// </summary>
    public int NumberOfColumns { get; set; }

    /// <summary>
    /// Target number of ship fields per row.
    /// </summary>
    public int[] TargetNumberOfShipFieldsPerRow { get; set; } = null!;

    /// <summary>
    /// Target number of ship fields per column
    /// </summary>
    public int[] TargetNumberOfShipFieldsPerColumn { get; set; } = null!;

    /// <summary>
    /// Target number of ships of a given length (e.g. [4,2,1] means 4 ships of length 1, 2 of length 2 and 1 of length 3)
    /// </summary>
    public int[] TargetNumberOfShipsPerLength { get; set; } = null!;

    /// <summary>
    /// Collection of set values (e.g. water). Not set fields can be omitted.
    /// </summary>
    public ICollection<GridValueDto> GridValues { get; set; } = new List<GridValueDto>();
}