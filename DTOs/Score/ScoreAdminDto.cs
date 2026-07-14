namespace ElearningAPI.DTOs.Score
{
    public class ScoreAdminDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = default!;
        public string QuizTitle { get; set; } = default!;
        public Guid SubjectId { get; set; }
        public Guid LevelId { get; set; }

        public float Score { get; set; }
        public int TimeUsedMinutes { get; set; }
        public DateTime TakenAt { get; set; }
    }
}
