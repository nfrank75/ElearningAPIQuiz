using System;
using ElearningAPI.Models.Quiz; // Pour SubjectType et LevelType

namespace ElearningAPI.Models.Epreuve
{
    public class Epreuve
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = default!;

        /// <summary>
        /// URL du fichier PDF stocké dans Azure Blob Storage.
        /// </summary>
        public string? PdfFile { get; set; }

        /// <summary>
        /// Indique si l'épreuve est corrigée ou non.
        /// </summary>
        public bool IsCorrected { get; set; }

        /// <summary>
        /// Année de l'épreuve (ex: 2023).
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// Matière de l'épreuve (Maths, Physique, etc.).
        /// </summary>
        public SubjectType Subject { get; set; }

        /// <summary>
        /// Niveau / classe (Form 1 → Upper Sixth).
        /// </summary>
        public LevelType Level { get; set; }
    }
}