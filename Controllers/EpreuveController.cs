using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElearningAPI.DTOs.Epreuve;
using ElearningAPI.Models.Epreuve;
using ElearningAPI.Models.Quiz;

namespace ElearningAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EpreuveController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EpreuveController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ------------------------------------------------------------
        // 1. UPLOAD EPREUVE (ADMIN)
        // Swagger-friendly: vrai champ "file"
        // ------------------------------------------------------------

        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadEpreuve(
            IFormFile file,
            [FromForm] string title,
            [FromForm] int? year,
            [FromForm] SubjectType subject,
            [FromForm] LevelType level,
            [FromForm] bool isCorrected)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Aucun fichier fourni.");

            var uploadsPath = Path.Combine(_env.ContentRootPath, "Uploads", "Epreuves");

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var publicUrl = $"/Uploads/Epreuves/{fileName}";

            var epreuve = new Epreuve
            {
                Title = title,
                Year = year,
                Subject = subject,
                Level = level,
                IsCorrected = isCorrected,
                PdfFile = publicUrl
            };

            _context.Epreuves.Add(epreuve);
            await _context.SaveChangesAsync();

            return Ok(new EpreuveResponseDto
            {
                Id = epreuve.Id,
                Title = epreuve.Title,
                Year = epreuve.Year,
                Subject = epreuve.Subject,
                Level = epreuve.Level,
                IsCorrected = epreuve.IsCorrected,
                PdfFile = publicUrl
            });
        }

        // ------------------------------------------------------------
        // 2. EPREUVES NON CORRIGÉES (PUBLIC)
        // ------------------------------------------------------------

        [HttpGet("uncorrected")]
        public async Task<IActionResult> GetNonCorrected(
            [FromQuery] SubjectType? subject,
            [FromQuery] LevelType? level)
        {
            var query = _context.Epreuves
                .Where(e => !e.IsCorrected)
                .AsQueryable();

            if (subject.HasValue)
                query = query.Where(e => e.Subject == subject.Value);

            if (level.HasValue)
                query = query.Where(e => e.Level == level.Value);

            return Ok(await query.ToListAsync());
        }

        // ------------------------------------------------------------
        // 3. EPREUVES CORRIGÉES (PUBLIC LIMITÉ À 5)
        // ------------------------------------------------------------

        [HttpGet("corrected/public")]
        public async Task<IActionResult> GetCorrectedPublic(
            [FromQuery] SubjectType? subject,
            [FromQuery] LevelType? level)
        {
            var query = _context.Epreuves
                .Where(e => e.IsCorrected)
                .AsQueryable();

            if (subject.HasValue)
                query = query.Where(e => e.Subject == subject.Value);

            if (level.HasValue)
                query = query.Where(e => e.Level == level.Value);

            return Ok(await query.Take(5).ToListAsync());
        }

        // ------------------------------------------------------------
        // 4. EPREUVES CORRIGÉES (PRIVÉ ILLIMITÉ)
        // ------------------------------------------------------------

        [HttpGet("corrected")]
        [Authorize]
        public async Task<IActionResult> GetCorrectedPrivate(
            [FromQuery] SubjectType? subject,
            [FromQuery] LevelType? level)
        {
            var query = _context.Epreuves
                .Where(e => e.IsCorrected)
                .AsQueryable();

            if (subject.HasValue)
                query = query.Where(e => e.Subject == subject.Value);

            if (level.HasValue)
                query = query.Where(e => e.Level == level.Value);

            return Ok(await query.ToListAsync());
        }
    }
}
