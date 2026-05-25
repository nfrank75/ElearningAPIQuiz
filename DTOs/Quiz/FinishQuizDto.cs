namespace ElearningAPI.DTOs.Quiz
{
    public class FinishQuizDto
    {
        public Guid QuizId { get; set; }
        public int TimeUsedMinutes { get; set; }
        public List<FinishQuizAnswerDto> Answers { get; set; } = new();
    }

    public class FinishQuizAnswerDto
    {
        public Guid QuestionId { get; set; }
        public string Answer { get; set; } = default!;
    }
}
