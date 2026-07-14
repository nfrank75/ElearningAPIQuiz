namespace ElearningAPI.DTOs.Epreuve
{
    public class EpreuveResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string PdfUrl { get; set; } = default!;
        public bool IsCorrected { get; set; }
        public int? Year { get; set; }

        public Guid SubjectId { get; set; }
        public Guid LevelId { get; set; }
    }
}
