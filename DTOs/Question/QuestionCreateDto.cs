// DTOs/Quiz/Question/QuestionCreateDto.cs
using System.Collections.Generic;
using ElearningAPI.Models.Quiz;

namespace ElearningAPI.DTOs.Quiz.Question
{
    /// <summary>
    /// DTO utilisé pour créer une question dans un quiz.
    /// Gère les QCM (plusieurs choix) et les questions Vrai/Faux.
    /// </summary>
    public class QuestionCreateDto
    {
        /// <summary>
        /// Énoncé de la question affiché à l'élève.
        /// </summary>
        public string Statement { get; set; } = default!;

        /// <summary>
        /// Type de question : QCM ou TrueFalse.
        /// </summary>
        public QuestionType Type { get; set; }

        /// <summary>
        /// Liste des choix possibles.
        /// - Pour un QCM : 3 à 6 choix (ex: A, B, C, D)
        /// - Pour TrueFalse : ["Vrai", "Faux"]
        /// </summary>
        public List<string> Options { get; set; } = new();

        /// <summary>
        /// La bonne réponse.
        /// Doit correspondre à l'un des éléments de Options.
        /// </summary>
        public string CorrectAnswer { get; set; } = default!;

        /// <summary>
        /// Explication affichée après correction.
        /// </summary>
        public string Explanation { get; set; } = default!;

        /// <summary>
        /// Nombre de points attribués à cette question.
        /// </summary>
        public float Points { get; set; }
    }
}