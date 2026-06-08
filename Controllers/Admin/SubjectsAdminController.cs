using ElearningAPI.DTOs.School;
using ElearningAPI.Models;
using ElearningAPI.Models.School;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearningAPI.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/subjects")]
    [Authorize(Roles = "Admin")]
    public class SubjectsAdminController : ControllerBase
    {
        private readonly AppDbContext _db;

        public SubjectsAdminController(AppDbContext db)
        {
            _db = db;
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> CreateSubject(CreateSubjectDto dto)
        {
            var subject = new Subject
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description
            };

            _db.Subjects.Add(subject);
            await _db.SaveChangesAsync();

            return Ok(subject);
        }

        //// GET ALL
        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetSubjects()
        //{
        //    var subjects = await _db.Subjects
        //        .OrderBy(s => s.Name)
        //        .ToListAsync();

        //    return Ok(subjects);
        //}

        //// GET ONE
        //[HttpGet("{id}")]

        //[AllowAnonymous]
        //public async Task<IActionResult> GetSubject(Guid id)
        //{
        //    var subject = await _db.Subjects.FindAsync(id);
        //    if (subject == null)
        //        return NotFound("Subject not found.");

        //    return Ok(subject);
        //}

        
        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(Guid id, UpdateSubjectDto dto)
        {
            var subject = await _db.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound("Subject not found.");

            subject.Name = dto.Name;
            subject.Description = dto.Description;

            await _db.SaveChangesAsync();
            return Ok(subject);
        }


        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(Guid id)
        {
            var subject = await _db.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound("Subject not found.");

            _db.Subjects.Remove(subject);
            await _db.SaveChangesAsync();

            return Ok("Subject deleted.");
        }
    }
}
