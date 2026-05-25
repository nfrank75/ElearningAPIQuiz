using ElearningAPI.Models;
using ElearningAPI.Models.Epreuve;
using ElearningAPI.Models.Quiz;
using ElearningAPI.Models.School;
using ElearningAPI.Models.Score;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // USERS
    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // QUIZ
    public DbSet<Quizzes> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }

    // SCORES
    public DbSet<Score> Scores { get; set; }
    public DbSet<GuestQuizAttempt> GuestQuizAttempts { get; set; }

    // EPREUVES
    public DbSet<Epreuve> Epreuves { get; set; }

    // NEW: Dynamic Levels & Subjects
    public DbSet<Level> Levels { get; set; }
    public DbSet<Subject> Subjects { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Convert List<string> Options to JSON
        modelBuilder.Entity<Question>()
            .Property(q => q.Options)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
            );

        base.OnModelCreating(modelBuilder);
    }
}
