using System;
using ElearningAPI.Models.Quiz;

namespace ElearningAPI.Models.Score
{
    public class Score
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public float Value { get; set; }
        public DateTime TakenAt { get; set; } = DateTime.UtcNow;
        public int TimeUsedMinutes { get; set; }
        public string AnswersJson { get; set; } = "{}";

        // Relations
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = default!;

        public Guid QuizId { get; set; }
        public Quizzes Quiz { get; set; } = default!;
    }
}