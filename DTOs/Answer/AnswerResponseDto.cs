// DTOs/Quiz/Answer/AnswerResponseDto.cs
using System;

namespace ElearningAPI.DTOs.Quiz.Answer
{
    /// <summary>
    /// DTO représentant une réponse possible à une question (si tu veux les gérer séparément).
    /// </summary>
    public class AnswerResponseDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = default!;
        public bool IsCorrect { get; set; }
    }
}