namespace ElearningAPI.Models
{
    public class Student : User
    {
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? SchoolName { get; set; }
        public string? ClassName { get; set; }

        public bool IsMember { get; set; } = false;

    }
}