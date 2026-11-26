namespace GameBackendAPI.DTOs.Registration
{
    public record RegistrationResponseDTO()
    {
        public required int Id { get; init; }
        public required int Rating { get; init; }
        public required string Username { get; init; }
        public required DateTimeOffset CreatedAt { get; init; }
    }
}