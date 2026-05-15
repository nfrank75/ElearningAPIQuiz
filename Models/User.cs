using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? FirstName { get; set; }

    public string? Email { get; set; }
    public string? Phone { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Student";
    public string UserType { get; set; } = "Student";

    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
