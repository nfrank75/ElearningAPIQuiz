using System.ComponentModel.DataAnnotations;

public class Student
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;

    public string SchoolName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;

    public string? Gender { get; set; }
    public int? BirthYear { get; set; }
    public string? FavoriteSubject { get; set; }

    public string Role { get; set; } = "Student";

    public string? AvatarUrl { get; set; }
    public bool IsMember { get; set; } = false;
}
