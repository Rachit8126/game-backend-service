using System.ComponentModel.DataAnnotations;

namespace GameBackendAPI.DTOs.Profile
{
    public record UpdateProfileRequestDTO
    {
        [EmailAddress, StringLength(254)]
        public string? Email { get; init; }

        [StringLength(50, MinimumLength = 3), RegularExpression("^[a-zA-Z0-9_]{3,50}$")]
        public string? Username { get; init; }
    }
}
