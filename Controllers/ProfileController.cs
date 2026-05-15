using ElearningAPI.DTOs;
using ElearningAPI.DTOs.Profile;
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
        private readonly IWebHostEnvironment _env;

        public ProfileController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // ---------------------------------------------------------
        //  GET PROFILE /api/Profile/me
        // ---------------------------------------------------------
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Invalid token");

            var guid = Guid.Parse(userId);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == guid);

            if (user == null)
                return NotFound("User not found");

            // -------------------------
            //   STUDENT PROFILE
            // -------------------------
            if (user.UserType == "Student")
            {
                var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == guid);
                if (student == null)
                    return NotFound("Student profile not found");

                return Ok(new
                {
                    userId = user.Id,
                    userType = user.UserType,
                    role = user.Role,

                    name = student.Name,
                    firstName = student.FirstName,
                    email = student.Email,
                    phone = student.Phone,

                    country = student.Country,
                    region = student.Region,
                    city = student.City,

                    schoolName = student.SchoolName,
                    className = student.ClassName,

                    gender = student.Gender,
                    birthYear = student.BirthYear,
                    favoriteSubject = student.FavoriteSubject,

                    avatar = student.AvatarUrl
                });
            }

            // -------------------------
            //   ADMIN PROFILE
            // -------------------------
            return Ok(new
            {
                userId = user.Id,
                userType = user.UserType,
                role = user.Role,

                name = user.Name,
                firstName = user.FirstName,
                email = user.Email,
                phone = user.Phone,
                avatar = user.AvatarUrl
            });
        }

        // ---------------------------------------------------------
        //  UPDATE PROFILE /api/Profile/update
        // ---------------------------------------------------------
        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile(ProfileUpdateDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Invalid token");

            var guid = Guid.Parse(userId);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == guid);

            if (user == null)
                return NotFound("User not found");

            // ---------------------------------------------------------
            //  STUDENT UPDATE (full profile)
            // ---------------------------------------------------------
            if (user.UserType == "Student")
            {
                var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == guid);
                if (student == null)
                    return NotFound("Student profile not found");

                // Student fields
                if (!string.IsNullOrWhiteSpace(dto.Name)) student.Name = dto.Name;
                if (!string.IsNullOrWhiteSpace(dto.FirstName)) student.FirstName = dto.FirstName;
                if (!string.IsNullOrWhiteSpace(dto.Phone)) student.Phone = dto.Phone;

                if (!string.IsNullOrWhiteSpace(dto.Country)) student.Country = dto.Country;
                if (!string.IsNullOrWhiteSpace(dto.Region)) student.Region = dto.Region;
                if (!string.IsNullOrWhiteSpace(dto.City)) student.City = dto.City;

                if (!string.IsNullOrWhiteSpace(dto.SchoolName)) student.SchoolName = dto.SchoolName;
                if (!string.IsNullOrWhiteSpace(dto.ClassName)) student.ClassName = dto.ClassName;

                if (!string.IsNullOrWhiteSpace(dto.Gender)) student.Gender = dto.Gender;
                if (dto.BirthYear.HasValue) student.BirthYear = dto.BirthYear;
                if (!string.IsNullOrWhiteSpace(dto.FavoriteSubject)) student.FavoriteSubject = dto.FavoriteSubject;

                // User fields
                if (!string.IsNullOrWhiteSpace(dto.Name)) user.Name = dto.Name;
                if (!string.IsNullOrWhiteSpace(dto.FirstName)) user.FirstName = dto.FirstName;
                if (!string.IsNullOrWhiteSpace(dto.Phone)) user.Phone = dto.Phone;

                await _db.SaveChangesAsync();

                return Ok(new
                {
                    message = "Profile updated successfully",
                    userId = user.Id,
                    userType = user.UserType,
                    role = user.Role,

                    name = student.Name,
                    firstName = student.FirstName,
                    email = student.Email,
                    phone = student.Phone,

                    country = student.Country,
                    region = student.Region,
                    city = student.City,

                    schoolName = student.SchoolName,
                    className = student.ClassName,

                    gender = student.Gender,
                    birthYear = student.BirthYear,
                    favoriteSubject = student.FavoriteSubject,

                    avatar = student.AvatarUrl
                });
            }

            // ---------------------------------------------------------
            //  ADMIN UPDATE (limited fields)
            // ---------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(dto.Name)) user.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.FirstName)) user.FirstName = dto.FirstName;
            if (!string.IsNullOrWhiteSpace(dto.Phone)) user.Phone = dto.Phone;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully",
                userId = user.Id,
                userType = user.UserType,
                role = user.Role,

                name = user.Name,
                firstName = user.FirstName,
                email = user.Email,
                phone = user.Phone,
                avatar = user.AvatarUrl
            });
        }

        // ---------------------------------------------------------
        //  UPDATE EMAIL /api/Profile/update-email
        // ---------------------------------------------------------
        [Authorize]
        [HttpPut("update-email")]
        public async Task<IActionResult> UpdateEmail(UpdateEmailDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.NewEmail))
                return BadRequest("Email is required");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Invalid token");

            var guid = Guid.Parse(userId);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == guid);

            if (user == null)
                return NotFound("User not found");

            if (await _db.Users.AnyAsync(u => u.Email == dto.NewEmail))
                return BadRequest("Email already exists");

            user.Email = dto.NewEmail;

            if (user.UserType == "Student")
            {
                var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == guid);
                student.Email = dto.NewEmail;
            }

            await _db.SaveChangesAsync();

            return Ok(new { message = "Email updated successfully", email = dto.NewEmail });
        }

        // ---------------------------------------------------------
        //  UPDATE PHONE /api/Profile/update-phone
        // ---------------------------------------------------------
        [Authorize]
        [HttpPut("update-phone")]
        public async Task<IActionResult> UpdatePhone(UpdatePhoneDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.NewPhone))
                return BadRequest("Phone is required");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Invalid token");

            var guid = Guid.Parse(userId);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == guid);

            if (user == null)
                return NotFound("User not found");

            if (await _db.Users.AnyAsync(u => u.Phone == dto.NewPhone))
                return BadRequest("Phone already exists");

            user.Phone = dto.NewPhone;

            if (user.UserType == "Student")
            {
                var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == guid);
                student.Phone = dto.NewPhone;
            }

            await _db.SaveChangesAsync();

            return Ok(new { message = "Phone updated successfully", phone = dto.NewPhone });
        }

        // ---------------------------------------------------------
        //  UPLOAD AVATAR /api/Profile/avatar
        // ---------------------------------------------------------
        [Authorize]
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Invalid token");

            var guid = Guid.Parse(userId);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == guid);

            if (user == null)
                return NotFound("User not found");

            var folder = Path.Combine(_env.ContentRootPath, "Uploads", "Avatars");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"{guid}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url = $"/Uploads/Avatars/{fileName}";

            user.AvatarUrl = url;

            if (user.UserType == "Student")
            {
                var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == guid);
                student.AvatarUrl = url;
            }

            await _db.SaveChangesAsync();

            return Ok(new { message = "Avatar updated successfully", avatar = url });
        }
    }
}
