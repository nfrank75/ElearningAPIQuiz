using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using ElearningAPI.DTOs.Epreuve;
using ElearningAPI.Models.Epreuve;
using ElearningAPI.Models.Quiz;

namespace ElearningAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EpreuveController : ControllerBase
    {
        private readonly AppDbContext  _context;
        private readonly BlobServiceClient _blobService;

        public EpreuveController(AppDbContext  context, BlobServiceClient blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // ------------------------------------------------------------
        // 1. UPLOAD EPREUVE (ADMIN)
        // ------------------------------------------------------------

        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadEpreuve(
            [FromForm] IFormFile file,
            [FromForm] EpreuveCreateDto dto)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Aucun fichier fourni.");

            var container = _blobService.GetBlobContainerClient("epreuves");
            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlobClient($"{Guid.NewGuid()}_{file.FileName}");
            await blob.UploadAsync(file.OpenReadStream(), overwrite: true);

            var epreuve = new Epreuve
            {
                Title = dto.Title,
                Year = dto.Year,
                Subject = dto.Subject,
                Level = dto.Level,
                IsCorrected = dto.IsCorrected,
                PdfFile = blob.Uri.ToString()
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
                PdfFile = epreuve.PdfFile
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