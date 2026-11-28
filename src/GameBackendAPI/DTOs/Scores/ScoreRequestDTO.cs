using System.ComponentModel.DataAnnotations;

namespace GameBackendAPI.DTOs.Scores
{
    public record ScoreRequestDTO
    {
        [Required, Range(0, 10000)]
        public int? Score { get; init; }
    }
}
