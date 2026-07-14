using System;
using System.Collections.Generic;

namespace ElearningAPI.Models.Quiz
{
    public class Question
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Statement { get; set; } = default!;
        public QuestionType Type { get; set; }

        // Stored as JSON in database
        public List<string> Options { get; set; } = new();

        public string CorrectAnswer { get; set; } = default!;
        public string Explanation { get; set; } = default!;
        public float Points { get; set; }

        // Foreign key
        public Guid QuizId { get; set; }
        public Quizzes Quiz { get; set; } = default!;

        // Navigation
        public List<Answer> Answers { get; set; } = new();
    }
}