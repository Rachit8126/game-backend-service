using GameBackendAPI.Data;
using GameBackendAPI.DTOs.Profile;
using GameBackendAPI.DTOs.Registration;
using GameBackendAPI.Entities;
using GameBackendAPI.Extensions;
using GameBackendAPI.Mappings;
using GameBackendAPI.Security.Hashing;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GameBackendAPI.Endpoints
{
    public static class PlayerEndpoints
    {
        public static void MapPlayerEndpoints(this IEndpointRouteBuilder app)
        {
            var mapGroup = app.MapGroup("/players").RequireAuthorization();
            mapGroup.MapPost("/", Register).AllowAnonymous();
            mapGroup.MapGet("/me", ReturnOwner);
            mapGroup.MapGet("/{id:int}", ReturnPublic);
            mapGroup.MapPatch("/me", Patch);
        }

        private static async Task<IResult> Patch(UpdateProfileRequestDTO updateProfileRequestDTO, ClaimsPrincipal user, PlayerContext db)
        {
            int? id = user.GetIntId();

            if (id == null)
            {
                return Results.Unauthorized();
            }

            var player = await db.Players.FirstOrDefaultAsync(p => p.Id == id);

            if (player == null)
            {
                return Results.NotFound();
            }

            player.Patch(updateProfileRequestDTO);

            try
            {
                await db.SaveChangesAsync();

                return Results.Ok(player.ToOwnerProfileDTO());
            }
            catch (DbUpdateException ex) when (ex.IsDuplicateEntryException())
            {
                return Results.Problem(statusCode: 409, title: "Duplicate Email or Username", detail: "User with this email or username already exists");
            }
        }

        private static async Task<IResult> ReturnPublic(int id, PlayerContext db)
        {
            var player = await db.Players.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            if (player == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(player.ToPublicProfileDTO());
        }

        private static async Task<IResult> ReturnOwner(ClaimsPrincipal user, PlayerContext db)
        {
            int? id = user.GetIntId();

            if (id == null)
            {
                return Results.Unauthorized();
            }

            var player = await db.Players.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

            if (player == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(player.ToOwnerProfileDTO());
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
            catch (DbUpdateException ex) when (ex.IsDuplicateEntryException())
            {
                return Results.Problem(statusCode: 409, title: "Duplicate Entry", detail: "Entry for this user already exists");
            }
        }
    }
}
