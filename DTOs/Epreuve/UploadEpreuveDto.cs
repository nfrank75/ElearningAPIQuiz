namespace ElearningAPI.DTOs.Epreuve
{
    public class UploadEpreuveDto
    {
        public string Title { get; set; } = default!;
        public int? Year { get; set; }
        public Guid SubjectId { get; set; }
        public Guid LevelId { get; set; }
        public bool IsCorrected { get; set; }
    }
}
