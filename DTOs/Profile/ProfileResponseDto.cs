namespace ElearningAPI.DTOs.Profile
{
    public class ProfileResponseDto
    {
        public Guid UserId { get; set; }
        public string UserType { get; set; }
        public string Role { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Student only
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? SchoolName { get; set; }
        public string? ClassName { get; set; }
    }
}
