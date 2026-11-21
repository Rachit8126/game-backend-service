namespace GameBackendAPI.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public int Rating { get; set; } = 0;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
