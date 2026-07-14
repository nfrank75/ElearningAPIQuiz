using ElearningAPI.Models.Quiz;

namespace ElearningAPI.DTOs.QuizAdmin
{
    public class AddQuestionDto
    {
        public string Statement { get; set; } = default!;
        public QuestionType Type { get; set; }
        public List<string> Options { get; set; } = new();
        public string CorrectAnswer { get; set; } = default!;
        public string Explanation { get; set; } = default!;
        public float Points { get; set; }
    }
}
