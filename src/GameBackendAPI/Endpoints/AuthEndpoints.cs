using GameBackendAPI.Data;
using GameBackendAPI.DTOs.Authentication;
using GameBackendAPI.Security.Authentication;
using GameBackendAPI.Security.Hashing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GameBackendAPI.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var mapGroup = app.MapGroup("/auth");

            mapGroup.MapPost("/login", Login)
                .Produces<LoginResponseDTO>()
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesValidationProblem()
                .WithSummary("Login users");
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
