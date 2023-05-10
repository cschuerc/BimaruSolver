using System.ComponentModel.DataAnnotations;

namespace webapi.Models;

public class GameDto
{
    [Range(6, 10)]
    public int NumberOfRows { get; set; }

    [Range(6, 10)]
    public int NumberOfColumns { get; set; }

    public int[] TargetNumberOfShipFieldsPerRow { get; set; } = null!;

    public int[] TargetNumberOfShipFieldsPerColumn { get; set; } = null!;

    public int[] TargetNumberOfShipsPerLength { get; set; } = null!;

    public ICollection<GridValueDto> GridValues { get; set; } = new List<GridValueDto>();
}