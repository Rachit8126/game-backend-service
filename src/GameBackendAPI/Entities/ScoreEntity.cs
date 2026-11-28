namespace GameBackendAPI.Entities
{
    public class ScoreEntity
    {
        public long Id { get; set; }
        public int PlayerId { get; set; }
        public int Score { get; set; }
        public DateTimeOffset SubmissionTime { get; set; }
    }
}
