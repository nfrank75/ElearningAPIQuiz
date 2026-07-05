namespace ElearningAPI.DTOs
{
    public class UpdatePhoneDto
    {
        public string CurrentPhone { get; set; } = default!;
        public string NewPhone { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
