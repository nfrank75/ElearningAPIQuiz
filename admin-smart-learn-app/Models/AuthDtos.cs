namespace AdminSmartLearn.Models
{
    public class LoginDto
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
    }

    public class AdminSignupDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
    }

    public class AuthResponseDto
    {
        public string? Message { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? Role { get; set; }
        public string? UserType { get; set; }
        public string? UserId { get; set; }
        public int ExpiresIn { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }
}
