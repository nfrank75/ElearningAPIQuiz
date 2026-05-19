namespace ElearningAPI.DTOs.Quiz
{
    public class QuizListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public int DurationMinutes { get; set; }
        public int QuestionsCount { get; set; }
    }
}
