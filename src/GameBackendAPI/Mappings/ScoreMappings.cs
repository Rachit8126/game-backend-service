using GameBackendAPI.DTOs.Scores;
using GameBackendAPI.Entities;

namespace GameBackendAPI.Mappings
{
    public static class ScoreMappings
    {
        public static ScoreResponseDTO ToResponseDTO(this ScoreEntity scoreEntity)
        {
            ScoreResponseDTO scoreResponseDTO = new()
            {
                Id = scoreEntity.Id,
                Score = scoreEntity.Score,
                SubmissionTime = scoreEntity.SubmissionTime
            };

            return scoreResponseDTO;
        }
    }
}
