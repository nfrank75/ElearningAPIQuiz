// DTOs/Quiz/QuizResponseDto.cs
using System;
using System.Collections.Generic;

namespace ElearningAPI.DTOs.Quiz
{
    /// <summary>
    /// DTO renvoyé au client lorsqu'on retourne un Quiz.
    /// On expose l'Id, les métadonnées, et éventuellement les questions.
    /// </summary>
    public class QuizResponseDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = default!;
        public int DurationMinutes { get; set; }
        public string Subject { get; set; } = default!;
        public string Level { get; set; } = default!;
        public float Coefficient { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Liste des questions du quiz.
        /// On utilise un DTO de question pour ne pas exposer l'entité EF.
        /// </summary>
        public List<Question.QuestionResponseDto> Questions { get; set; } = new();
    }
}