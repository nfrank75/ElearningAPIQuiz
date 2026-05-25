namespace ElearningAPI.DTOs.School
{
    public class UpdateLevelDto
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public int Order { get; set; }
    }
}
