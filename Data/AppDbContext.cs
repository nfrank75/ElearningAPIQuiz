using ElearningAPI.Models;
using ElearningAPI.Models.Epreuve;
using ElearningAPI.Models.Quiz;
using ElearningAPI.Models.Score;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Admin> Admins { get; set; }

    public DbSet<Epreuve> Epreuves { get; set; }

    public DbSet<Quizzes> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<Score> Scores { get; set; }
    public DbSet<GuestQuizAttempt> GuestQuizAttempts { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // IMPORTANT : PAS DE DISCRIMINATOR
        // Student et Admin ne dérivent plus de User

        // VALUE CONVERTER pour Options (List<string>)
        modelBuilder.Entity<Question>()
            .Property(q => q.Options)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
            );

        base.OnModelCreating(modelBuilder);
    }
}
