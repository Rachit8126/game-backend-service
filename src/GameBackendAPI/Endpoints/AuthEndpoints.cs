using GameBackendAPI.Data;
using GameBackendAPI.DTOs.Authentication;
using GameBackendAPI.Security.Authentication;
using GameBackendAPI.Security.Hashing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace GameBackendAPI.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var mapGroup = app.MapGroup("/auth");
            mapGroup.MapPost("/login", Login);
            mapGroup.MapGet("/me", TestAuthEndpoint).RequireAuthorization();
        }

        private static IResult TestAuthEndpoint(ClaimsPrincipal claimsPrincipal)
        {
            var sub = claimsPrincipal.Claims.FirstOrDefault(p => p.Type == JwtRegisteredClaimNames.Sub);
            var username = claimsPrincipal.Claims.FirstOrDefault(p => p.Type == "username");

            return Results.Ok(new List<string>() { sub?.Value, username?.Value });
        }

        private static async Task<IResult> Login(LoginRequestDTO loginRequestDTO, PlayerContext db, ICustomPasswordHasher passwordHasher, IAuthTokenService authTokenService, IOptions<AuthSettings> authSettings)
        {
            var player = await db.Players.AsNoTracking().FirstOrDefaultAsync(p => p.Email == loginRequestDTO.Email);

            bool isValid = passwordHasher.Verify(loginRequestDTO.Password, player?.HashedPassword);

            if (!isValid || player == null)
            {
                return Results.Problem(statusCode: 401, title: "Login Failed", detail: "Invalid email or password");
            }

            string token = authTokenService.CreateToken(player);

            LoginResponseDTO loginResponseDTO = new()
            {
                Token = token,
                TokenType = "Bearer",
                ExpiryInSeconds = authSettings.Value.LifeTimeInSeconds
            };

            return Results.Ok(loginResponseDTO);
        }
    }
}
