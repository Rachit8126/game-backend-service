using GameBackendAPI.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace GameBackendAPI.Security.Authentication
{
    public class AuthTokenService(IOptions<AuthSettings> options) : IAuthTokenService
    {
        private readonly AuthSettings _authSettings = options.Value;

        public string CreateToken(PlayerEntity player)
        {
            SymmetricSecurityKey symmetricSecurityKey = _authSettings.GetSymmetricSecurityKey();
            SigningCredentials signingCredentials = new(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>() {
                new(JwtRegisteredClaimNames.Sub, player.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("username", player.Username)
            };

            ClaimsIdentity claimsIdentity = new(claims);

            SecurityTokenDescriptor securityTokenDescriptor = new()
            {
                Subject = claimsIdentity,
                SigningCredentials = signingCredentials,
                Issuer = _authSettings.Issuer,
                Audience = _authSettings.Audience,
                Expires = DateTime.UtcNow.AddSeconds(_authSettings.LifeTimeInSeconds)
            };

            var token = new JsonWebTokenHandler().CreateToken(securityTokenDescriptor);

            return token;
        }
    }
}
