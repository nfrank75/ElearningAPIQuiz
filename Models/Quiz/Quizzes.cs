using System;
using System.Collections.Generic;
using ElearningAPI.Models.School;

namespace ElearningAPI.Models.Quiz
{
    public class Quizzes
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = default!;
        public int DurationMinutes { get; set; }

        // NEW dynamic subject
        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; } = default!;

        // NEW dynamic level
        public Guid LevelId { get; set; }
        public Level Level { get; set; } = default!;

        public float Coefficient { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Question> Questions { get; set; } = new();
    }
}
