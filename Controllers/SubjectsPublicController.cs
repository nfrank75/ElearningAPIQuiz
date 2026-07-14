using ElearningAPI.Models.School;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearningAPI.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    public class SubjectsPublicController : ControllerBase
    {
        private readonly AppDbContext _db;

        public SubjectsPublicController(AppDbContext db)
        {
            _db = db;
        }

        // GET /api/subjects
        [HttpGet]
        public async Task<IActionResult> GetSubjects()
        {
            var subjects = await _db.Subjects.ToListAsync();
            return Ok(subjects);
        }

        // GET /api/subjects/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubject(Guid id)
        {
            var subject = await _db.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound("Subject not found");

            return Ok(subject);
        }
    }
}
