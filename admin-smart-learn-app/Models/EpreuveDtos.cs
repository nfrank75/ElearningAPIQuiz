namespace AdminSmartLearn.Models
{
    public class EpreuveDto
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public int Year { get; set; }
        public bool IsCorrected { get; set; }
        public string? FileUrl { get; set; }
        public string? FilePath { get; set; }
        public string? Url { get; set; }
        public string? PdfUrl { get; set; }
        public string? SubjectId { get; set; }
        public string? LevelId { get; set; }
    }

    public class EpreuveUploadDto
    {
        public string? Title { get; set; }
        public int Year { get; set; }
        public string? SubjectId { get; set; }
        public string? LevelId { get; set; }
        public bool IsCorrected { get; set; }
        // The file is handled via IFormFile in the controller parameter directly.
    }
}
