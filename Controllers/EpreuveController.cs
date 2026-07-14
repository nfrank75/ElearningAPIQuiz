using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElearningAPI.DTOs.Epreuve;
using ElearningAPI.Models.Epreuve;
using ElearningAPI.Models.School;

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

        // ----------------------------------------------------
        // 1. UPLOAD EPREUVE (ADMIN)
        // ----------------------------------------------------
        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadEpreuve(
            IFormFile file,
            [FromForm] string title,
            [FromForm] int? year,
            [FromForm] Guid subjectId,
            [FromForm] Guid levelId,
            [FromForm] bool isCorrected)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Aucun fichier fourni.");

            // Validate subject
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null)
                return BadRequest("Invalid subjectId");

            // Validate level
            var level = await _context.Levels.FindAsync(levelId);
            if (level == null)
                return BadRequest("Invalid levelId");

            // Path: /var/www/elearning-api/Uploads/Epreuves/
            var uploadsPath = Path.Combine(_env.ContentRootPath, "Uploads", "Epreuves");

            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var safeTitle = title.Trim().Replace(" ", "_");
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}_{safeTitle}{extension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            var publicUrl = $"/Uploads/Epreuves/{fileName}";

            var epreuve = new Epreuve
            {
                Title = title,
                Year = year,
                SubjectId = subjectId,
                LevelId = levelId,
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
                PdfUrl = publicUrl,
                IsCorrected = epreuve.IsCorrected,

                SubjectId = subject.Id,

                LevelId = level.Id
            });
        }

        // ----------------------------------------------------
        // 2. GET UNCORRECTED EPREUVES (PUBLIC)
        // ----------------------------------------------------
        [HttpGet("uncorrected")]
        public async Task<IActionResult> GetUncorrected(
            [FromQuery] Guid? subjectId,
            [FromQuery] Guid? levelId,
            [FromQuery] int? year)
        {
            var query = _context.Epreuves
                .Include(e => e.Subject)
                .Include(e => e.Level)
                .Where(e => !e.IsCorrected)
                .AsQueryable();

            if (subjectId.HasValue)
                query = query.Where(e => e.SubjectId == subjectId.Value);

            if (levelId.HasValue)
                query = query.Where(e => e.LevelId == levelId.Value);

            if (year.HasValue)
                query = query.Where(e => e.Year == year.Value);

                var list = await query
        .OrderByDescending(e => e.Year)
        .Select(e => new EpreuveResponseDto
        {
            Id = e.Id,
            Title = e.Title,
            PdfUrl = e.PdfFile!,
            IsCorrected = e.IsCorrected,
            Year = e.Year,

            SubjectId = e.SubjectId,

            LevelId = e.LevelId
        })
        .ToListAsync();

                return Ok(list);
        }

        // ----------------------------------------------------
        // 3. GET CORRECTED EPREUVES (PUBLIC, LIMIT 5)
        // ----------------------------------------------------
        [HttpGet("corrected/public")]
        public async Task<IActionResult> GetCorrectedPublic()
        {
                var list = await _context.Epreuves
        .Include(e => e.Subject)
        .Include(e => e.Level)
        .Where(e => e.IsCorrected)
        .OrderByDescending(e => e.Year)
        .Take(5)
        .Select(e => new EpreuveResponseDto
        {
            Id = e.Id,
            Title = e.Title,
            PdfUrl = e.PdfFile!,
            IsCorrected = e.IsCorrected,
            Year = e.Year,

            SubjectId = e.SubjectId,

            LevelId = e.LevelId
        })
        .ToListAsync();

                return Ok(list);
            }

        // ----------------------------------------------------
        // 4. GET CORRECTED EPREUVES (PRIVATE, FULL LIST)
        // ----------------------------------------------------
        [HttpGet("corrected")]
        [Authorize]
        public async Task<IActionResult> GetCorrectedPrivate()
        {
                var list = await _context.Epreuves
        .Include(e => e.Subject)
        .Include(e => e.Level)
        .Where(e => e.IsCorrected)
        .OrderByDescending(e => e.Year)
        .Select(e => new EpreuveResponseDto
        {
            Id = e.Id,
            Title = e.Title,
            PdfUrl = e.PdfFile!,
            IsCorrected = e.IsCorrected,
            Year = e.Year,

            SubjectId = e.SubjectId,

            LevelId = e.LevelId
        })
        .ToListAsync();

                return Ok(list);
        }
    }
}
