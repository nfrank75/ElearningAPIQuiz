namespace ElearningAPI.DTOs.Users
{
    public class UserFullDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public string Role { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        // Student-specific
        public string? Country { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }

        public string? SchoolName { get; set; }
        public string? ClassName { get; set; }

        public string? Gender { get; set; }
        public int? BirthYear { get; set; }
        public string? FavoriteSubject { get; set; }

        public bool? IsMember { get; set; }

        public bool? IsActive { get; set; }
    }
}
