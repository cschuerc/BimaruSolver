namespace Bimaru.Interface.Database
{
    public class GameEntity
    {
        public int Id { get; set; }

        public GameSize Size { get; set; }

        public GameDifficulty Difficulty { get; set; }

        public int NumberOfRows { get; set; }

        public int NumberOfColumns { get; set; }

        public int[] TargetNumberOfShipFieldsPerRow { get; set; } = null!;

        public int[] TargetNumberOfShipFieldsPerColumn { get; set; } = null!;

        public int[] TargetNumberOfShipsPerLength { get; set; } = null!;

        public ICollection<GridValueEntity> GridValues { get; set; } = new List<GridValueEntity>();

        /// <summary>
        /// This ID will be the same for equivalent games and (almost) always different for different games.
        /// Together with a unique constraint, this property prevents having equivalent games in the database.
        /// </summary>
        public int AlmostUniqueGameId { get; set; }
    }
}