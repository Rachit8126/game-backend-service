using GameBackendAPI.Data;
using GameBackendAPI.DTOs;
using GameBackendAPI.Entities;
using GameBackendAPI.Mappings;
using GameBackendAPI.Security;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace GameBackendAPI.Endpoints
{
    public static class PlayerEndpoints
    {
        public static void MapPlayerEndpoints(this IEndpointRouteBuilder app)
        {
            var mapGroup = app.MapGroup("/players");
            mapGroup.MapPost("/", Register);
        }

        private static async Task<IResult> Register(RegistrationRequestDTO registrationRequestDTO, PlayerContext db, ICustomPasswordHasher passwordHasher)
        {
            string hashedPassword = passwordHasher.GetHash(registrationRequestDTO.Password);

            Player player = new()
            {
                Username = registrationRequestDTO.Username,
                Email = registrationRequestDTO.Email,
                HashedPassword = hashedPassword
            };

            db.Players.Add(player);

            try
            {
                await db.SaveChangesAsync();

                RegistrationResponseDTO registrationResponseDTO = player.ToRegistrationDTO();

                return Results.Created($"/players/{player.Id}", registrationResponseDTO);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
            {
                return Results.Problem(statusCode: 409, title: "Duplicate Entry", detail: "Entry for this user already exists");
            }
        }
    }
}
