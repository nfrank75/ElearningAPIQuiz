using ElearningAPI.DTOs.School;
using ElearningAPI.Models;
using ElearningAPI.Models.School;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearningAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/levels")]
    [Authorize(Roles = "Admin")]
    public class LevelsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public LevelsController(AppDbContext db)
        {
            _db = db;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CreateLevel(CreateLevelDto dto)
        {
            var level = new Level
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Order = dto.Order
            };

            _db.Levels.Add(level);
            await _db.SaveChangesAsync();

            return Ok(level);
        }

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> GetLevels()
        {
            var levels = await _db.Levels
                .OrderBy(l => l.Order)
                .ToListAsync();

            return Ok(levels);
        }

        // GET ONE
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLevel(Guid id)
        {
            var level = await _db.Levels.FindAsync(id);
            if (level == null)
                return NotFound("Level not found.");

            return Ok(level);
        }

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLevel(Guid id, UpdateLevelDto dto)
        {
            var level = await _db.Levels.FindAsync(id);
            if (level == null)
                return NotFound("Level not found.");

            level.Name = dto.Name;
            level.Description = dto.Description;
            level.Order = dto.Order;

            await _db.SaveChangesAsync();
            return Ok(level);
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLevel(Guid id)
        {
            var level = await _db.Levels.FindAsync(id);
            if (level == null)
                return NotFound("Level not found.");

            _db.Levels.Remove(level);
            await _db.SaveChangesAsync();

            return Ok("Level deleted.");
        }
    }
}
