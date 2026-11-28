using GameBackendAPI.Entities;

namespace GameBackendAPI.Security.Authentication
{
    public interface IAuthTokenService
    {
        public string CreateToken(PlayerEntity player);
    }
}
