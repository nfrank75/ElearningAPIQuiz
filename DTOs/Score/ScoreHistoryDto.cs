namespace ElearningAPI.DTOs.Score
{
    public class ScoreHistoryDto
    {
        public Guid ScoreId { get; set; }
        public Guid QuizId { get; set; }
        public string QuizTitle { get; set; } = default!;
        public Guid SubjectId { get; set; }
        public Guid LevelId { get; set; }
        public float Value { get; set; }
        public int TimeUsedMinutes { get; set; }
        public DateTime TakenAt { get; set; }
    }
}
