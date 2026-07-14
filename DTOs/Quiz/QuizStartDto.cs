namespace ElearningAPI.DTOs.Quiz
{
    public class QuizStartDto
    {
        public Guid QuizId { get; set; }
        public string Title { get; set; } = default!;
        public int DurationMinutes { get; set; }
        public List<QuizQuestionDto> Questions { get; set; } = new();
    }
}
