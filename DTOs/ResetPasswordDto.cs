namespace ElearningAPI.DTOs
{
    public class ResetPasswordDto
    {
        public string Otp { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
    }
}
