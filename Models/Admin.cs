using System.ComponentModel.DataAnnotations;

namespace ElearningAPI.Models
{
    public class Admin
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string PasswordHash { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }


        public string Role { get; set; } = "Admin";

    }
}
