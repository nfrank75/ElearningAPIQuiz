using System;
using ElearningAPI.Models.Quiz;

namespace ElearningAPI.DTOs.Epreuve
{
    /// <summary>
    /// DTO renvoyé au client pour représenter une épreuve.
    /// </summary>
    public class EpreuveResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string? PdfFile { get; set; }
        public bool IsCorrected { get; set; }
        public int? Year { get; set; }
        public SubjectType Subject { get; set; }
        public LevelType Level { get; set; }
    }
}