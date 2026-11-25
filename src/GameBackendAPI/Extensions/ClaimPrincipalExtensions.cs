using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace GameBackendAPI.Extensions
{
    public static class ClaimPrincipalExtensions
    {
        public static int? GetIntId(this ClaimsPrincipal claimPrincipal)
        {
            string? id = claimPrincipal.Claims.FirstOrDefault(p => p.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (!int.TryParse(id, out int intId))
            {
                return null;
            }

            return intId;
        }
    }
}
