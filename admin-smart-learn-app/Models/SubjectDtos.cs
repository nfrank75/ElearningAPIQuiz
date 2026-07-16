namespace AdminSmartLearn.Models
{
    public class SubjectDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class CreateSubjectDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateSubjectDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
