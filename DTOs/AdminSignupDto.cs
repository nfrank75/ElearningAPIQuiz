namespace ElearningAPI.DTOs
{
    public class AdminSignupDto
    {
        public string Name { get; set; } = default!;
        public string? Email { get; set; }     // optional
        public string? Phone { get; set; }     // optional
        public string Password { get; set; } = default!;
    }
}