using System.ComponentModel.DataAnnotations;

namespace webapi.Models;

public class GameDto
{
    [Range(6, 10)]
    public int NumberOfRows { get; set; }

    [Range(6, 10)]
    public int NumberOfColumns { get; set; }

    [Required]
    public int[] TargetNumberOfShipFieldsPerRow { get; set; } = null!;

    [Required]
    public int[] TargetNumberOfShipFieldsPerColumn { get; set; } = null!;

    [Required]
    public int[] TargetNumberOfShipsPerLength { get; set; } = null!;

    [Required]
    public ICollection<GridValueDto> GridValues { get; set; } = new List<GridValueDto>();
}