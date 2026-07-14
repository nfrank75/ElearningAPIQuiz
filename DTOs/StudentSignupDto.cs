namespace ElearningAPI.DTOs
{
    public class StudentSignupDto
    {
        public string Name { get; set; } = default!;
        public string? Email { get; set; }     // optional
        public string? Phone { get; set; }     // optional
        public string Password { get; set; } = default!;

        public string? Country { get; set; }
        public string? City { get; set; }
        public string? SchoolName { get; set; }
        public string? ClassName { get; set; }

    }
}