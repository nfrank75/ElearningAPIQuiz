namespace AdminSmartLearn.Models
{
    public class ProfileUpdateDto
    {
        public string? Name { get; set; }
        public string? FirstName { get; set; }
        public string? Phone { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? SchoolName { get; set; }
        public string? ClassName { get; set; }
        public string? Gender { get; set; }
        public int? BirthYear { get; set; }
        public string? FavoriteSubject { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Avatar { get; set; }
        public string? PhotoUrl { get; set; }
    }

    public class UpdateEmailDto
    {
        public string? NewEmail { get; set; }
    }

    public class UpdatePhoneDto
    {
        public string? NewPhone { get; set; }
    }
}
