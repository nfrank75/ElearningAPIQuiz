using System;
using System.Collections.Generic;

namespace ElearningAPI.Models.Quiz
{
    public class Quizzes
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = default!;
        public int DurationMinutes { get; set; }   // 30 or 60
        public SubjectType Subject { get; set; }
        public LevelType Level { get; set; } // Form 1 → Upper Sixth
        public float Coefficient { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public List<Question> Questions { get; set; } = new();
    }
}