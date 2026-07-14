namespace ElearningAPI.DTOs.QuizAdmin
{
    public class QuizStatsDto
    {
        public Guid QuizId { get; set; }
        public string Title { get; set; } = default!;
        public int TotalAttempts { get; set; }
        public float AverageScore { get; set; }
        public float MaxScore { get; set; }
        public float MinScore { get; set; }
        public float SuccessRate { get; set; }
        public string Difficulty { get; set; } = default!;
        public Dictionary<string, int> ScoreDistribution { get; set; } = new();
        public int AverageTimeUsed { get; set; }
    }
}
