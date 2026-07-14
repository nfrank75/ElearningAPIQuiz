// DTOs/Quiz/Question/QuestionResponseDto.cs
using System;
using System.Collections.Generic;
using ElearningAPI.Models.Quiz;

namespace ElearningAPI.DTOs.Quiz.Question
{
    /// <summary>
    /// DTO renvoyé au client pour représenter une question.
    /// </summary>
    public class QuestionResponseDto
    {
        public Guid Id { get; set; }

        public string Statement { get; set; } = default!;
        public QuestionType Type { get; set; }
        public List<string> Options { get; set; } = new();
        public string Explanation { get; set; } = default!;
        public float Points { get; set; }
    }
}