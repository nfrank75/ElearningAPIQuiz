namespace AdminSmartLearn.Models
{
    public class QuizDto
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public int DurationMinutes { get; set; }
        public string? SubjectId { get; set; }
        public string? LevelId { get; set; }
        public double Coefficient { get; set; }
        public bool IsActive { get; set; }
        public List<QuestionDto>? Questions { get; set; }
    }

    public class QuizCreateDto
    {
        public string? Title { get; set; }
        public int DurationMinutes { get; set; }
        public string? SubjectId { get; set; }
        public string? LevelId { get; set; }
        public double Coefficient { get; set; }
    }

    public class QuestionDto
    {
        public string? Id { get; set; }
        public string? Statement { get; set; }
        public int Type { get; set; } // 0 = QCM, 1 = QCU, etc. based on QuestionType enum
        public List<string>? Options { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public double Points { get; set; }
    }

    public class AddQuestionDto
    {
        public string? Statement { get; set; }
        public int Type { get; set; }
        public List<string>? Options { get; set; }
        public string? CorrectAnswer { get; set; }
        public string? Explanation { get; set; }
        public double Points { get; set; }
    }

    public class UpdateQuestionDto : AddQuestionDto
    {
    }
}
