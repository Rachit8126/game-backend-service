using GameBackendAPI.Data;
using GameBackendAPI.DTOs.Scores;
using GameBackendAPI.Entities;
using GameBackendAPI.Extensions;
using GameBackendAPI.Mappings;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GameBackendAPI.Endpoints
{
    public static class ScoreEndpoints
    {
        public static void MapScoreEndpoints(this IEndpointRouteBuilder app)
        {
            var mapGroup = app.MapGroup("/scores").RequireAuthorization();

            mapGroup.MapPost("/", SubmitScore)
                .Produces<ScoreResponseDTO>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesValidationProblem()
                .WithSummary("Submit Score");
        }

        private static async Task<IResult> SubmitScore(ScoreRequestDTO scoreRequestDTO, DatabaseContext db, ClaimsPrincipal user)
        {
            int? playerId = user.GetIntId();

            if (playerId == null)
            {
                return Results.Unauthorized();
            }

            if (scoreRequestDTO.Score == null)
            {
                return Results.BadRequest();
            }

            ScoreEntity scoreEntity = new()
            {
                PlayerId = playerId.Value,
                Score = scoreRequestDTO.Score.Value,
            };

            db.Scores.Add(scoreEntity);

            try
            {
                await db.SaveChangesAsync();

                ScoreResponseDTO scoreResponseDTO = scoreEntity.ToResponseDTO();

                return Results.Created("", scoreResponseDTO);
            }
            catch (DbUpdateException ex) when (ex.IsForeignKeyViolationException())
            {
                return Results.NotFound();
            }
        }
    }
}
