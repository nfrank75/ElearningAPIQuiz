using ElearningAPI.Models.Quiz;

namespace ElearningAPI.DTOs.QuizAdmin
{
    public class CreateQuizDto
    {
        public string Title { get; set; } = default!;
        public int DurationMinutes { get; set; }
        public SubjectType Subject { get; set; }
        public LevelType Level { get; set; }
        public float Coefficient { get; set; }
    }
}
