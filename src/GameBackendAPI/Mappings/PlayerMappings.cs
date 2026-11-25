using GameBackendAPI.DTOs.Profile;
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

        public static PublicProfileDTO ToPublicProfileDTO(this Player player)
        {
            PublicProfileDTO publicProfileDTO = new()
            {
                Id = player.Id,
                Rating = player.Rating,
                Username = player.Username
            };

            return publicProfileDTO;
        }

        public static OwnerProfileDTO ToOwnerProfileDTO(this Player player)
        {
            OwnerProfileDTO ownerProfileDTO = new()
            {
                Id = player.Id,
                Rating = player.Rating,
                Username = player.Username,
                Email = player.Email
            };

            return ownerProfileDTO;
        }

        public static void Patch(this Player player, UpdateProfileRequestDTO updateProfileRequestDTO)
        {
            if (updateProfileRequestDTO.Email != null)
            {
                player.Email = updateProfileRequestDTO.Email;
            }

            if (updateProfileRequestDTO.Username != null)
            {
                player.Username = updateProfileRequestDTO.Username;
            }
        }
    }
}
