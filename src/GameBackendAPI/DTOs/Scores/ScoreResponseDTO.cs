namespace GameBackendAPI.DTOs.Scores
{
    public record ScoreResponseDTO
    {
        public required long Id { get; init; }
        public required int Score { get; init; }
        public required DateTimeOffset SubmissionTime { get; init; }
    }
}
