namespace AdminSmartLearn.Models
{
    public class ScoreDto
    {
        public string? Id { get; set; }
        public string? QuizId { get; set; }
        public string? QuizTitle { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public double Value { get; set; }
        public double MaxValue { get; set; }
        public DateTime Date { get; set; }
    }
}
