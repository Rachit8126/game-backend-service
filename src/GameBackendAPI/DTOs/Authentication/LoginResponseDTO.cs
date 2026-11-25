namespace GameBackendAPI.DTOs.Authentication
{
    public record LoginResponseDTO
    {
        public required string Token { get; init; }

        public required string TokenType { get; init; }

        public int ExpiryInSeconds { get; init; }
    }
}
