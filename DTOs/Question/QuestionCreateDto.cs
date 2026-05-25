// DTOs/Quiz/Question/QuestionCreateDto.cs
using System.Collections.Generic;
using ElearningAPI.Models.Quiz;

namespace ElearningAPI.DTOs.Quiz.Question
{
    public class QuestionCreateDto
    {
        public string Statement { get; set; } = default!;

        public QuestionType Type { get; set; }

        public List<string> Options { get; set; } = new();

        public string CorrectAnswer { get; set; } = default!;

        public string Explanation { get; set; } = default!;

        public float Points { get; set; }
    }
}