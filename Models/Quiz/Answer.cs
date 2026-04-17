using System;

namespace ElearningAPI.Models.Quiz
{
    public class Answer
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Text { get; set; } = default!;
        public bool IsCorrect { get; set; }

        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = default!;
    }
}