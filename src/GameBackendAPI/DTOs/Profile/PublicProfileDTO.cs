namespace GameBackendAPI.DTOs.Profile
{
    public record PublicProfileDTO
    {
        public required int Id { get; init; }
        public required int Rating { get; init; }
        public required string Username { get; init; }
    }
}
