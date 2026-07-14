using ElearningAPI.DTOs.Score;
using ElearningAPI.Models.Score;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearningAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScoreController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ScoreController(AppDbContext db)
        {
            _db = db;
        }

        // ----------------------------------------------------
        // 1. GET MY SCORES (Student only)
        // ----------------------------------------------------
        [Authorize(Roles = "Student")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyScores()
        {

            var sub = User.FindFirst("sub")?.Value;

            if (sub == null)
                return Unauthorized("Invalid token: missing 'sub' claim.");

            var studentId = Guid.Parse(sub);

            var scores = await _db.Scores
                .Where(s => s.StudentId == studentId)
                .Include(s => s.Quiz)
                .OrderByDescending(s => s.TakenAt)
                .Select(s => new ScoreHistoryDto
                {
                    ScoreId = s.Id,
                    QuizId = s.QuizId,
                    QuizTitle = s.Quiz.Title,
                    SubjectId = s.Quiz.SubjectId,
                    LevelId = s.Quiz.LevelId,
                    Value = s.Value,
                    TimeUsedMinutes = s.TimeUsedMinutes,
                    TakenAt = s.TakenAt
                })
                .ToListAsync();

            return Ok(scores);
        }

        // ----------------------------------------------------
        // 2. GET SCORES BY QUIZ (Admin only)
        // ----------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpGet("quiz/{quizId}")]
        public async Task<IActionResult> GetScoresByQuiz(Guid quizId)
        {
            var scores = await _db.Scores
                .Where(s => s.QuizId == quizId)
                .Include(s => s.Student)
                .OrderByDescending(s => s.Value)
                .Select(s => new ScoreByQuizDto
                {
                    StudentId = s.StudentId,
                    StudentName = s.Student.Name,
                    Score = s.Value,
                    TimeUsedMinutes = s.TimeUsedMinutes,
                    TakenAt = s.TakenAt
                })
                .ToListAsync();

            return Ok(scores);
        }

        // ----------------------------------------------------
        // 3. GET ALL SCORES (Admin only)
        // ----------------------------------------------------
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllScores()
        {
            var scores = await _db.Scores
                .Include(s => s.Student)
                .Include(s => s.Quiz)
                .OrderByDescending(s => s.TakenAt)
                .Select(s => new ScoreAdminDto
                {
                    StudentId = s.StudentId,
                    StudentName = s.Student.Name,
                    QuizTitle = s.Quiz.Title,
                    SubjectId = s.Quiz.SubjectId,
                    LevelId = s.Quiz.LevelId,
                    Score = s.Value,
                    TimeUsedMinutes = s.TimeUsedMinutes,
                    TakenAt = s.TakenAt
                })
                .ToListAsync();

            return Ok(scores);
        }
    }
}
