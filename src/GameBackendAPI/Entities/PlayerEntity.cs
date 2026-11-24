namespace GameBackendAPI.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public int Rating { get; set; } = 0;
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string HashedPassword { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
