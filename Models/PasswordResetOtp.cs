namespace ElearningAPI.Models
{
    public class PasswordResetOtp
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string OtpHash { get; set; } = default!;
        public DateTime ExpiresAt { get; set; } 
        public bool Used { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
