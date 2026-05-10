using System;
using ElearningAPI.Models.Quiz;

namespace ElearningAPI.DTOs.Epreuve
{
    public class EpreuveResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string PdfUrl { get; set; } = default!;
        public bool IsCorrected { get; set; }
        public int? Year { get; set; }
        public SubjectType Subject { get; set; }
        public LevelType Level { get; set; }
        public string SubjectName => Subject.ToString();
        public string LevelName => Level.ToString();
    }
}
