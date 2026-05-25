namespace ElearningAPI.DTOs.Quiz
{
    public class QuizQuestionDto
    {
        public Guid QuestionId { get; set; }
        public string Statement { get; set; } = default!;
        public List<string> Options { get; set; } = new();
    }
}
