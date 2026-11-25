using GameBackendAPI.DTOs.Registration;
using GameBackendAPI.Entities;

namespace GameBackendAPI.Mappings
{
    public static class PlayerMappings
    {
        public static RegistrationResponseDTO ToRegistrationDTO(this Player player)
        {
            RegistrationResponseDTO registrationResponseDTO = new()
            {
                Id = player.Id,
                Username = player.Username,
                Rating = player.Rating,
                CreatedAt = player.CreatedAt
            };

            return registrationResponseDTO;
        }
    }
}
