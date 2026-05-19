public class GuestQuizAttempt
{
    public int Id { get; set; }
    public string IpAddress { get; set; } = default!;
    public Guid QuizId { get; set; }
    public DateTime TakenAt { get; set; }
}
