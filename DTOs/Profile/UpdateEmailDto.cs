namespace ElearningAPI.DTOs
{
    public class UpdateEmailDto
    {
        public string CurrentEmail { get; set; } = default!;
        public string NewEmail { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
