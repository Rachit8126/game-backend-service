using System.ComponentModel.DataAnnotations;

namespace GameBackendAPI.Security.Authentication
{
    public class AuthSettings
    {
        [Range(30, 86400)]
        public int LifeTimeInSeconds { get; set; }

        [Required]
        public required string Issuer { get; set; }

        [Required]
        public required string Audience { get; set; }

        [Required, MinLength(32)]
        public required string Key { get; set; }
    }
}
