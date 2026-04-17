using ElearningAPI.Models.Quiz;

namespace ElearningAPI.DTOs.Epreuve
{
    /// <summary>
    /// DTO utilisé pour créer une épreuve (métadonnées).
    /// Le fichier PDF est envoyé séparément via IFormFile.
    /// </summary>
    public class EpreuveCreateDto
    {
        public string Title { get; set; } = default!;
        public int? Year { get; set; }
        public SubjectType Subject { get; set; }
        public LevelType Level { get; set; }
        public bool IsCorrected { get; set; }
    }
}