using System.ComponentModel.DataAnnotations;

namespace ElearningAPI.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Student";

        public string UserType { get; set; } = "Student";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
