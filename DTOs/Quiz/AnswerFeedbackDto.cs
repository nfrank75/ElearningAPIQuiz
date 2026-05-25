namespace ElearningAPI.DTOs.Quiz
{
    public class AnswerFeedbackDto
    {
        public bool Correct { get; set; }
        public string Explanation { get; set; } = default!;
    }
}
