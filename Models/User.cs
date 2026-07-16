using System.ComponentModel.DataAnnotations;

public class User
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? FirstName { get; set; }

    public string? Email { get; set; } = default!;
    public string? Phone { get; set; } = default!;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Student";
    public string UserType { get; set; } = "Student";

    public string? AvatarUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
