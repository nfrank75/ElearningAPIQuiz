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
        /// <summary>
        /// Titre du quiz (ex: "Maths Form 3 - Fractions").
        /// </summary>
        public string Title { get; set; } = default!;

        /// <summary>
        /// Durée du quiz en minutes (ex: 30 ou 60).
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Matière concernée (ex: "Maths", "Physics").
        /// </summary>
        public string Subject { get; set; } = default!;

        /// <summary>
        /// Niveau (ex: "Form 1", "Upper Sixth").
        /// </summary>
        public string Level { get; set; } = default!;

        /// <summary>
        /// Coefficient du quiz (impact sur la moyenne).
        /// </summary>
        public float Coefficient { get; set; }
    }
}