// DTOs/Quiz/QuizCreateDto.cs
using System;

namespace ElearningAPI.DTOs.Quiz
{
    /// <summary>
    /// DTO utilisé pour créer un nouveau Quiz.
    /// On ne met ici que les champs que le client a le droit de fournir.
    /// </summary>
    public class QuizCreateDto
    {
        public string Title { get; set; } = default!;
        public int DurationMinutes { get; set; }
        public Guid? SubjectId { get; set; }
        public Guid? LevelId { get; set; }
        public float Coefficient { get; set; }
    }
}