namespace ElearningAPI.Models.School
{
    public class Level
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = default!;
        public int Order { get; set; }
        public string? Description { get; set; }
    }
}
