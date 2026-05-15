using ElearningAPI.DTOs;
using ElearningAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ElearningAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ProfileController(AppDbContext db)
        {
            _db = db;
        }

        // ---------------- UPDATE PROFILE ----------------
        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile(ProfileUpdateDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request");

            // Récupérer l'ID utilisateur depuis le JWT
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (userId == null)
                return Unauthorized("Invalid token");

            // Charger l'utilisateur
            var guid = Guid.Parse(userId);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == guid);

            if (user == null)
                return NotFound("User not found");

            // -------------------------
            //   UPDATE STUDENT
            // -------------------------
            if (user.UserType == "Student")
            {
                var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == user.Id);

                if (student == null)
                    return NotFound("Student profile not found");

                // Mise à jour Student
                if (!string.IsNullOrWhiteSpace(dto.Name)) student.Name = dto.Name;
                if (!string.IsNullOrWhiteSpace(dto.Phone)) student.Phone = dto.Phone;
                if (!string.IsNullOrWhiteSpace(dto.Country)) student.Country = dto.Country;
                if (!string.IsNullOrWhiteSpace(dto.City)) student.City = dto.City;
                if (!string.IsNullOrWhiteSpace(dto.SchoolName)) student.SchoolName = dto.SchoolName;
                if (!string.IsNullOrWhiteSpace(dto.ClassName)) student.ClassName = dto.ClassName;

                // Mise à jour Users
                if (!string.IsNullOrWhiteSpace(dto.Name)) user.Name = dto.Name;
                if (!string.IsNullOrWhiteSpace(dto.Phone)) user.Phone = dto.Phone;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Profile updated successfully",
                    userId = user.Id,
                    userType = user.UserType,
                    role = user.Role,

                    // Infos Student
                    name = student.Name,
                    email = student.Email,
                    phone = student.Phone,
                    country = student.Country,
                    city = student.City,
                    schoolName = student.SchoolName,
                    className = student.ClassName
                });
            }

            // -------------------------
            //   UPDATE ADMIN
            // -------------------------
            if (!string.IsNullOrWhiteSpace(dto.Name)) user.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Phone)) user.Phone = dto.Phone;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully",
                userId = user.Id,
                userType = user.UserType,
                role = user.Role,

                // Infos Admin
                name = user.Name,
                email = user.Email,
                phone = user.Phone
            });
        }
    }
}
