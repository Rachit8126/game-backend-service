using System.ComponentModel.DataAnnotations;

namespace GameBackendAPI.DTOs.Authentication
{
    public record LoginRequestDTO
    {
        [Required]
        public required string Email { get; init; }

        [Required]
        public required string Password { get; init; }
    }
}
