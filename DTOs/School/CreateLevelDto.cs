namespace ElearningAPI.DTOs.School
{
    public class CreateLevelDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public int Order { get; set; }
    }
}
