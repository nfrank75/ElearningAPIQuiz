using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElearningAPI.DTOs.Users;
using ElearningAPI.Models;

namespace ElearningAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------
        // 1. GET ALL USERS (ADMIN)
        // ----------------------------------------------------
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            // ADMIN USERS
            var admins = await _context.Users
                .Select(u => new UserFullDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    FirstName = u.FirstName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role,
                    UserType = u.UserType,
                    AvatarUrl = u.AvatarUrl,
                    CreatedAt = u.CreatedAt,
                    IsActive = u.IsActive,

                    // Champs Student mis à null pour les admins
                    Country = null,
                    Region = null,
                    City = null,
                    SchoolName = null,
                    ClassName = null,
                    Gender = null,
                    BirthYear = null,
                    FavoriteSubject = null,
                    IsMember = null
                })
                .ToListAsync();

            // STUDENTS
            var students = await _context.Students
                .Select(s => new UserFullDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    FirstName = s.FirstName,
                    Email = s.Email,
                    Phone = s.Phone,
                    Role = s.Role,
                    UserType = "Student",
                    AvatarUrl = s.AvatarUrl,
                    CreatedAt = DateTime.UtcNow, // Students don't have CreatedAt
                    IsActive = s.IsActive,

                    Country = s.Country,
                    Region = s.Region,
                    City = s.City,

                    SchoolName = s.SchoolName,
                    ClassName = s.ClassName,

                    Gender = s.Gender,
                    BirthYear = s.BirthYear,
                    FavoriteSubject = s.FavoriteSubject,
                    IsMember = s.IsMember
                })
                .ToListAsync();

            // Fusion des deux listes
            var allUsers = admins.Concat(students)
                                 .OrderBy(u => u.Name)
                                 .ToList();

            return Ok(allUsers);
        }

        // ----------------------------------------------------
        // 2. DELETE USER (ADMIN)
        // ----------------------------------------------------
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            // Empêcher un admin de supprimer son propre compte
            var currentUserId = User.FindFirst("sub")?.Value;
            if (currentUserId != null && Guid.Parse(currentUserId) == id)
                return BadRequest("You cannot delete your own admin account.");

            // Vérifier si c'est un Admin
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (admin != null)
            {
                _context.Users.Remove(admin);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Admin user deleted successfully" });
            }

            // Vérifier si c'est un Student
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Student deleted successfully" });
            }

            return NotFound("User not found");
        }

        // ----------------------------------------------------
        // 3. DISABLE USER (ADMIN)
        // ----------------------------------------------------
        [HttpPut("{id}/disable")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DisableUser(Guid id)
        {
            // Empêcher un admin de se désactiver lui-même
            var currentUserId = User.FindFirst("sub")?.Value;
            if (currentUserId != null && Guid.Parse(currentUserId) == id)
                return BadRequest("You cannot disable your own admin account.");

            // Vérifier si c'est un Admin
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (admin != null)
            {
                admin.IsActive = false;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Admin account disabled successfully" });
            }

            // Vérifier si c'est un Student
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student != null)
            {
                student.IsActive = false;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Student account disabled successfully" });
            }

            return NotFound("User not found");
        }

        // ----------------------------------------------------
        // 4. ENABLE USER (ADMIN)
        // ----------------------------------------------------
        [HttpPut("{id}/enable")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnableUser(Guid id)
        {
            // Vérifier si c'est un Admin
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (admin != null)
            {
                admin.IsActive = true;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Admin account enabled successfully" });
            }

            // Vérifier si c'est un Student
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student != null)
            {
                student.IsActive = true;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Student account enabled successfully" });
            }

            return NotFound("User not found");
        }
    }
}
