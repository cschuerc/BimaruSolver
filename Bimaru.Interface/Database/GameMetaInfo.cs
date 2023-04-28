namespace Bimaru.Interface.Database
{
    /// <summary>
    /// Meta information about a Bimaru game.
    /// Equality based on all data fields.
    /// </summary>
    public readonly struct GameMetaInfo
    {
        public GameMetaInfo(int id, GameSize size, GameDifficulty difficulty)
        {
            Id = id;
            Size = size;
            Difficulty = difficulty;
        }

        public int Id { get; }

        public GameSize Size { get; }

        public GameDifficulty Difficulty { get; }
    }
}
