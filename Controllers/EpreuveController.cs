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

        // UPLOAD (ADMIN)
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

            // path : /var/www/elearning-api/Uploads/Epreuves/
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
                PdfUrl = publicUrl
            });
        }

        // Not corrected
        [HttpGet("uncorrected")]
        public async Task<IActionResult> GetUncorrected(
            [FromQuery] SubjectType? subject,
            [FromQuery] LevelType? level,
            [FromQuery] int? year)
        {
            var query = _context.Epreuves.Where(e => !e.IsCorrected);

            if (subject.HasValue)
                query = query.Where(e => e.Subject == subject.Value);

            if (level.HasValue)
                query = query.Where(e => e.Level == level.Value);

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
                    Subject = e.Subject,
                    Level = e.Level
                })
                .ToListAsync();

            return Ok(list);
        }

        // Corrected (PUBLIC limited at 5)
        [HttpGet("corrected/public")]
        public async Task<IActionResult> GetCorrectedPublic()
        {
            var list = await _context.Epreuves
                .Where(e => e.IsCorrected)
                .Take(5)
                .Select(e => new EpreuveResponseDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    PdfUrl = e.PdfFile!,
                    IsCorrected = e.IsCorrected,
                    Year = e.Year,
                    Subject = e.Subject,
                    Level = e.Level
                })
                .ToListAsync();

            return Ok(list);
        }

        // CORRECTED (PRIVATE)
        [HttpGet("corrected")]
        [Authorize]
        public async Task<IActionResult> GetCorrectedPrivate()
        {
            var list = await _context.Epreuves
                .Where(e => e.IsCorrected)
                .Select(e => new EpreuveResponseDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    PdfUrl = e.PdfFile!,
                    IsCorrected = e.IsCorrected,
                    Year = e.Year,
                    Subject = e.Subject,
                    Level = e.Level
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
