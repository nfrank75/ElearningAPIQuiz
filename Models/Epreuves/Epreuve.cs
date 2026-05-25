using System;
using ElearningAPI.Models.School;

namespace ElearningAPI.Models.Epreuve
{
    public class Epreuve
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = default!;
        public string? PdfFile { get; set; }
        public bool IsCorrected { get; set; }
        public int? Year { get; set; }

        // NEW dynamic subject
        public Guid SubjectId { get; set; }
        public Subject Subject { get; set; } = default!;

        // NEW dynamic level
        public Guid LevelId { get; set; }
        public Level Level { get; set; } = default!;
    }
}
