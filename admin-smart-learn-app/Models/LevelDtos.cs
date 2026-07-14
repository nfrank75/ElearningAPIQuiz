namespace AdminSmartLearn.Models
{
    public class LevelDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
    }

    public class CreateLevelDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
    }

    public class UpdateLevelDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Order { get; set; }
    }
}
