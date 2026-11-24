using System.ComponentModel.DataAnnotations;

namespace GameBackendAPI.DTOs
{
    public record RegistrationRequestDTO()
    {
        [Required, EmailAddress, StringLength(254)]
        public required string Email { get; init; }

        [Required, StringLength(maximumLength: 128, MinimumLength = 8)]
        public required string Password { get; init; }

        [Required, StringLength(50, MinimumLength = 3), RegularExpression("^[a-zA-Z0-9_]{3,50}$")]
        public required string Username { get; init; }
    }

    public record RegistrationResponseDTO()
    {
        public required int Id { get; init; }
        public required int Rating { get; init; }
        public required string Username { get; init; }
        public required DateTimeOffset CreatedAt { get; init; }
    }
}