using ElearningAPI.DTOs.Quiz;
using ElearningAPI.DTOs.QuizAdmin;
using ElearningAPI.Models.Quiz;
using ElearningAPI.Models.School;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearningAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/quiz")]
    [Authorize(Roles = "Admin")]
    public class QuizAdminController : ControllerBase
    {
        private readonly AppDbContext _db;

        public QuizAdminController(AppDbContext db)
        {
            _db = db;
        }

        // ----------------------------------------------------
        // 1. CREATE QUIZ
        // ----------------------------------------------------
        [HttpPost("create")]
        public async Task<IActionResult> CreateQuiz(QuizCreateDto dto)
        {
            if (dto.SubjectId == null)
                return BadRequest("subjectId is required");

            if (dto.LevelId == null)
                return BadRequest("levelId is required");

            var subject = await _db.Subjects.FindAsync(dto.SubjectId);
            if (subject == null)
                return BadRequest("Invalid subjectId");

            var level = await _db.Levels.FindAsync(dto.LevelId);
            if (level == null)
                return BadRequest("Invalid levelId");

            var quiz = new Quizzes
            {
                Title = dto.Title,
                DurationMinutes = dto.DurationMinutes,
                SubjectId = dto.SubjectId.Value,
                LevelId = dto.LevelId.Value,
                Coefficient = dto.Coefficient,
                IsActive = true
            };

            _db.Quizzes.Add(quiz);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Quiz created", quizId = quiz.Id });
        }

        // ----------------------------------------------------
        // 2. ADD QUESTION
        // ----------------------------------------------------
        [HttpPost("{quizId}/add-question")]
        public async Task<IActionResult> AddQuestion(Guid quizId, AddQuestionDto dto)
        {
            var quiz = await _db.Quizzes.FindAsync(quizId);
            if (quiz == null)
                return NotFound("Quiz not found");

            var question = new Question
            {
                QuizId = quizId,
                Statement = dto.Statement,
                Type = dto.Type,
                Options = dto.Options,
                CorrectAnswer = dto.CorrectAnswer,
                Explanation = dto.Explanation,
                Points = dto.Points
            };

            _db.Questions.Add(question);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Question added", questionId = question.Id });
        }

        // ----------------------------------------------------
        // 3. UPDATE QUESTION
        // ----------------------------------------------------
        [HttpPut("question/{questionId}")]
        public async Task<IActionResult> UpdateQuestion(Guid questionId, UpdateQuestionDto dto)
        {
            var question = await _db.Questions.FindAsync(questionId);
            if (question == null)
                return NotFound("Question not found");

            question.Statement = dto.Statement;
            question.Type = dto.Type;
            question.Options = dto.Options;
            question.CorrectAnswer = dto.CorrectAnswer;
            question.Explanation = dto.Explanation;
            question.Points = dto.Points;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Question updated" });
        }

        // ----------------------------------------------------
        // 4. DELETE QUESTION
        // ----------------------------------------------------
        [HttpDelete("question/{questionId}")]
        public async Task<IActionResult> DeleteQuestion(Guid questionId)
        {
            var question = await _db.Questions.FindAsync(questionId);
            if (question == null)
                return NotFound("Question not found");

            _db.Questions.Remove(question);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Question deleted" });
        }

        // ----------------------------------------------------
        // 5. TOGGLE QUIZ
        // ----------------------------------------------------
        [HttpPut("{quizId}/toggle")]
        public async Task<IActionResult> ToggleQuiz(Guid quizId)
        {
            var quiz = await _db.Quizzes.FindAsync(quizId);
            if (quiz == null)
                return NotFound("Quiz not found");

            quiz.IsActive = !quiz.IsActive;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Quiz status updated", isActive = quiz.IsActive });
        }

        // ----------------------------------------------------
        // 6. DELETE QUIZ
        // ----------------------------------------------------
        [HttpDelete("{quizId}")]
        public async Task<IActionResult> DeleteQuiz(Guid quizId)
        {
            var quiz = await _db.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null)
                return NotFound("Quiz not found");

            _db.Questions.RemoveRange(quiz.Questions);
            _db.Quizzes.Remove(quiz);

            await _db.SaveChangesAsync();

            return Ok(new { message = "Quiz deleted" });
        }

        // ----------------------------------------------------
        // 7. QUIZ STATS
        // ----------------------------------------------------
        [HttpGet("{quizId}/stats")]
        public async Task<IActionResult> GetQuizStats(Guid quizId)
        {
            var quiz = await _db.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null)
                return NotFound("Quiz not found");

            var scores = await _db.Scores
                .Where(s => s.QuizId == quizId)
                .ToListAsync();

            if (!scores.Any())
                return Ok(new QuizStatsDto
                {
                    QuizId = quizId,
                    Title = quiz.Title,
                    TotalAttempts = 0,
                    AverageScore = 0,
                    MaxScore = 0,
                    MinScore = 0,
                    SuccessRate = 0,
                    Difficulty = "Unknown",
                    ScoreDistribution = new Dictionary<string, int>(),
                    AverageTimeUsed = 0
                });

            int total = scores.Count;
            float avg = scores.Average(s => s.Value);
            float max = scores.Max(s => s.Value);
            float min = scores.Min(s => s.Value);
            float successRate = scores.Count(s => s.Value >= 50) * 100f / total;

            string difficulty =
                avg >= 80 ? "Very Easy" :
                avg >= 60 ? "Easy" :
                avg >= 40 ? "Medium" :
                avg >= 20 ? "Hard" :
                "Very Hard";

            var distribution = new Dictionary<string, int>
            {
                { "0-50", scores.Count(s => s.Value < 50) },
                { "50-70", scores.Count(s => s.Value >= 50 && s.Value < 70) },
                { "70-90", scores.Count(s => s.Value >= 70 && s.Value < 90) },
                { "90-100", scores.Count(s => s.Value >= 90) }
            };

            int avgTime = (int)scores.Average(s => s.TimeUsedMinutes);

            return Ok(new QuizStatsDto
            {
                QuizId = quizId,
                Title = quiz.Title,
                TotalAttempts = total,
                AverageScore = avg,
                MaxScore = max,
                MinScore = min,
                SuccessRate = successRate,
                Difficulty = difficulty,
                ScoreDistribution = distribution,
                AverageTimeUsed = avgTime
            });
        }
    }
}
