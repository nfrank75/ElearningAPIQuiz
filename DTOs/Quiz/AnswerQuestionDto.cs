namespace ElearningAPI.DTOs.Quiz
{
    public class AnswerQuestionDto
    {
        public Guid QuestionId { get; set; }
        public string Answer { get; set; } = default!;
    }
}
