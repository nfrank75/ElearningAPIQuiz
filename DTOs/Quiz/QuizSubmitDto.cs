// DTOs/Quiz/QuizSubmitDto.cs
using System;
using System.Collections.Generic;

namespace ElearningAPI.DTOs.Quiz
{
    /// <summary>
    /// DTO utilisé par l'élève pour soumettre ses réponses à un quiz.
    /// </summary>
    public class QuizSubmitDto
    {
        /// <summary>
        /// Identifiant du quiz passé.
        /// </summary>
        public Guid QuizId { get; set; }

        /// <summary>
        /// Temps utilisé par l'élève en minutes.
        /// Si le temps expire, le frontend envoie la durée maximale.
        /// </summary>
        public int TimeUsedMinutes { get; set; }

        /// <summary>
        /// Liste des réponses de l'élève.
        /// Chaque élément correspond à une question.
        /// </summary>
        public List<StudentAnswerDto> Answers { get; set; } = new();
    }

    /// <summary>
    /// Réponse d'un élève à une question.
    /// </summary>
    public class StudentAnswerDto
    {
        public Guid QuestionId { get; set; }

        /// <summary>
        /// Réponse choisie par l'élève.
        /// Doit correspondre à l'une des options de la question.
        /// </summary>
        public string SelectedAnswer { get; set; } = default!;
    }
}