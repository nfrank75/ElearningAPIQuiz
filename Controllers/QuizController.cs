using ElearningAPI.DTOs.Quiz;
using ElearningAPI.Models;
using ElearningAPI.Models.Quiz;
using ElearningAPI.Models.Score;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearningAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly AppDbContext _db;

        public QuizController(AppDbContext db)
        {
            _db = db;
        }

        // -----------------------------
        // 1. GET LEVELS (public)
        // -----------------------------
        [HttpGet("levels")]
        public IActionResult GetLevels()
        {
            var levels = Enum.GetNames(typeof(LevelType));
            return Ok(levels);
        }

        // -----------------------------
        // 2. GET SUBJECTS (public)
        // -----------------------------
        [HttpGet("subjects")]
        public IActionResult GetSubjects()
        {
            var subjects = Enum.GetNames(typeof(SubjectType));
            return Ok(subjects);
        }

        // -----------------------------
        // 3. GET QUIZZES BY LEVEL + SUBJECT (public)
        // -----------------------------
        [HttpGet]
        public async Task<IActionResult> GetQuizzes([FromQuery] LevelType level, [FromQuery] SubjectType subject)
        {
            var quizzes = await _db.Quizzes
                .Where(q => q.Level == level && q.Subject == subject && q.IsActive)
                .Select(q => new QuizListDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    DurationMinutes = q.DurationMinutes,
                    QuestionsCount = q.Questions.Count
                })
                .ToListAsync();

            return Ok(quizzes);
        }

        // -----------------------------
        // 4. START QUIZ (public)
        // -----------------------------
        [HttpPost("start/{quizId}")]
        public async Task<IActionResult> StartQuiz(Guid quizId)
        {
            var quiz = await _db.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null)
                return NotFound("Quiz not found");

            // Limite 3 quiz pour non connectés
            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;

            if (!isAuthenticated)
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                int count = await _db.GuestQuizAttempts
                    .CountAsync(a => a.IpAddress == ip);

                if (count >= 3)
                    return Unauthorized("You must login to continue playing quizzes");

                _db.GuestQuizAttempts.Add(new GuestQuizAttempt
                {
                    IpAddress = ip,
                    QuizId = quizId,
                    TakenAt = DateTime.UtcNow
                });

                await _db.SaveChangesAsync();
            }

            return Ok(new QuizStartDto
            {
                QuizId = quiz.Id,
                Title = quiz.Title,
                DurationMinutes = quiz.DurationMinutes,
                Questions = quiz.Questions.Select(q => new QuizQuestionDto
                {
                    QuestionId = q.Id,
                    Statement = q.Statement,
                    Options = q.Options
                }).ToList()
            });
        }

        // -----------------------------
        // 5. ANSWER QUESTION (public)
        // -----------------------------
        [HttpPost("answer")]
        public async Task<IActionResult> AnswerQuestion(AnswerQuestionDto dto)
        {
            var question = await _db.Questions.FirstOrDefaultAsync(q => q.Id == dto.QuestionId);

            if (question == null)
                return NotFound("Question not found");

            bool correct = question.CorrectAnswer.Trim().ToLower() ==
                           dto.Answer.Trim().ToLower();

            return Ok(new AnswerFeedbackDto
            {
                Correct = correct,
                Explanation = question.Explanation
            });
        }

        // -----------------------------
        // 6. FINISH QUIZ (Student only)
        // -----------------------------
        [Authorize]
        [HttpPost("finish")]
        public async Task<IActionResult> FinishQuiz(FinishQuizDto dto)
        {
            var quiz = await _db.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == dto.QuizId);

            if (quiz == null)
                return NotFound("Quiz not found");

            foreach (var c in User.Claims)
            {
                Console.WriteLine($"CLAIM: {c.Type} = {c.Value}");
            }

            var userId = Guid.Parse(
                User.FindFirst("sub")?.Value ??
                User.FindFirst("nameid")?.Value ??
                User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ??
                throw new Exception("User ID claim not found in token")
            );

            int correct = 0;
            int wrong = 0;

            foreach (var answer in dto.Answers)
            {
                var q = quiz.Questions.FirstOrDefault(x => x.Id == answer.QuestionId);
                if (q == null) continue;

                if (!string.IsNullOrWhiteSpace(q.CorrectAnswer) &&
                    !string.IsNullOrWhiteSpace(answer.Answer) &&
                    q.CorrectAnswer.Trim().ToLower() == answer.Answer.Trim().ToLower())
                    correct++;
                else
                    wrong++;
            }

            float scoreValue = quiz.Questions.Count == 0
                ? 0
                : (float)correct / quiz.Questions.Count * 100;

            var score = new Score
            {
                StudentId = userId,
                QuizId = quiz.Id,
                Value = scoreValue,
                TimeUsedMinutes = dto.TimeUsedMinutes,
                AnswersJson = System.Text.Json.JsonSerializer.Serialize(dto.Answers)
            };

            _db.Scores.Add(score);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                correctAnswers = correct,
                wrongAnswers = wrong,
                score = scoreValue
            });
        }
    }
}
