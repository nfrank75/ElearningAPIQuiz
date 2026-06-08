using ElearningAPI.Models.School;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearningAPI.Controllers
{
    [ApiController]
    [Route("api/levels")]
    public class LevelsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public LevelsController(AppDbContext db)
        {
            _db = db;
        }

        // GET /api/levels
        [HttpGet]
        public async Task<IActionResult> GetLevels()
        {
            var levels = await _db.Levels.ToListAsync();
            return Ok(levels);
        }

        // GET /api/levels/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLevel(Guid id)
        {
            var level = await _db.Levels.FindAsync(id);
            if (level == null)
                return NotFound("Level not found");

            return Ok(level);
        }
    }
}
