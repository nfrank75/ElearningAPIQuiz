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

            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null)
                return BadRequest("Invalid subjectId");

            var level = await _context.Levels.FindAsync(levelId);
            if (level == null)
                return BadRequest("Invalid levelId");

            var uploadsPath = Path.Combine(_env.ContentRootPath, "Uploads", "Epreuves");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            // FORMATAGE DU NOM DE FICHIER
            var safeTitle = title.Trim().Replace(" ", "_");
            var safeSubject = subject.Name.Trim().Replace(" ", "_");
            var safeLevel = level.Name.Trim().Replace(" ", "_");
            var safeYear = year.HasValue ? year.Value.ToString() : "Sans_Annee";
            var correctedLabel = isCorrected ? "Corrigé" : "Non_Corrigé";
            var extension = Path.GetExtension(file.FileName);

            var fileName = $"{safeTitle}_{safeYear}_{safeLevel}_{safeSubject}_{correctedLabel}{extension}";
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
        // 4. GET CORRECTED EPREUVES (PRIVATE)
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

        // ----------------------------------------------------
        // 5. DELETE EPREUVE (ADMIN)
        // ----------------------------------------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEpreuve(Guid id)
        {
            var epreuve = await _context.Epreuves.FirstOrDefaultAsync(e => e.Id == id);
            if (epreuve == null)
                return NotFound("Epreuve not found");

            if (!string.IsNullOrWhiteSpace(epreuve.PdfFile))
            {
                var filePath = Path.Combine(_env.ContentRootPath, epreuve.PdfFile.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.Epreuves.Remove(epreuve);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Epreuve deleted successfully" });
        }

        // ----------------------------------------------------
        // 6. UPDATE EPREUVE (ADMIN)
        // ----------------------------------------------------
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateEpreuve(Guid id, [FromForm] UpdateEpreuveDto dto)
        {
            var epreuve = await _context.Epreuves.FirstOrDefaultAsync(e => e.Id == id);
            if (epreuve == null)
                return NotFound("Epreuve not found");

            var subject = await _context.Subjects.FindAsync(dto.SubjectId);
            if (subject == null)
                return BadRequest("Invalid subjectId");

            var level = await _context.Levels.FindAsync(dto.LevelId);
            if (level == null)
                return BadRequest("Invalid levelId");

            epreuve.Title = dto.Title;
            epreuve.Year = dto.Year;
            epreuve.SubjectId = dto.SubjectId;
            epreuve.LevelId = dto.LevelId;
            epreuve.IsCorrected = dto.IsCorrected;

            if (dto.File != null)
            {
                if (!string.IsNullOrWhiteSpace(epreuve.PdfFile))
                {
                    var oldPath = Path.Combine(_env.ContentRootPath, epreuve.PdfFile.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var uploadsPath = Path.Combine(_env.ContentRootPath, "Uploads", "Epreuves");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                // FORMATAGE DU NOM DE FICHIER
                var safeTitle = dto.Title.Trim().Replace(" ", "_");
                var safeSubject = subject.Name.Trim().Replace(" ", "_");
                var safeLevel = level.Name.Trim().Replace(" ", "_");
                var safeYear = dto.Year.HasValue ? dto.Year.Value.ToString() : "Sans_Annee";
                var correctedLabel = dto.IsCorrected ? "Corrected" : "Not_Corrected";
                var extension = Path.GetExtension(dto.File.FileName);

                var fileName = $"{safeTitle}_{safeYear}_{safeLevel}_{safeSubject}_{correctedLabel}{extension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await dto.File.CopyToAsync(stream);

                epreuve.PdfFile = $"/Uploads/Epreuves/{fileName}";
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Epreuve updated successfully",
                epreuve = new EpreuveResponseDto
                {
                    Id = epreuve.Id,
                    Title = epreuve.Title,
                    Year = epreuve.Year,
                    PdfUrl = epreuve.PdfFile!,
                    IsCorrected = epreuve.IsCorrected,
                    SubjectId = epreuve.SubjectId,
                    LevelId = epreuve.LevelId
                }
            });
        }
    }
}
