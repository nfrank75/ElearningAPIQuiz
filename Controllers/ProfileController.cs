using ElearningAPI.DTOs;
using ElearningAPI.DTOs.Profile;
using ElearningAPI.Helpers;
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
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Lire l'ID utilisateur (logique unifiée)
            var userId =
                User.FindFirstValue("sub") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Invalid token.");

            var guid = Guid.Parse(userId);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == guid);
            if (user == null)
                return NotFound("User not found.");

            // ---------------------------------------------------------
            //  STUDENT UPDATE (full profile)
            // ---------------------------------------------------------
            if (user.UserType == "Student")
            {
                var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == guid);
                if (student == null)
                    return NotFound("Student profile not found.");

                // Student fields
                if (!string.IsNullOrWhiteSpace(dto.Name)) student.Name = dto.Name;
                if (!string.IsNullOrWhiteSpace(dto.FirstName)) student.FirstName = dto.FirstName;

                if (!string.IsNullOrWhiteSpace(dto.Country)) student.Country = dto.Country;
                if (!string.IsNullOrWhiteSpace(dto.Region)) student.Region = dto.Region;
                if (!string.IsNullOrWhiteSpace(dto.City)) student.City = dto.City;

                if (!string.IsNullOrWhiteSpace(dto.SchoolName)) student.SchoolName = dto.SchoolName;
                if (!string.IsNullOrWhiteSpace(dto.ClassName)) student.ClassName = dto.ClassName;

                if (!string.IsNullOrWhiteSpace(dto.Gender)) student.Gender = dto.Gender;
                if (dto.BirthYear.HasValue) student.BirthYear = dto.BirthYear;
                if (!string.IsNullOrWhiteSpace(dto.FavoriteSubject)) student.FavoriteSubject = dto.FavoriteSubject;

                // User fields (synchronisation)
                if (!string.IsNullOrWhiteSpace(dto.Name)) user.Name = dto.Name;
                if (!string.IsNullOrWhiteSpace(dto.FirstName)) user.FirstName = dto.FirstName;

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
                avatar = user.AvatarUrl
            });
        }

        // ---------------------------------------------------------
        //  UPDATE EMAIL /api/Profile/update-email
        // ---------------------------------------------------------

        [Authorize]
        [HttpPut("update-email")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId =
                User.FindFirstValue("sub") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Invalid token.");

            var guid = Guid.Parse(userId);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == guid);
            if (user == null)
                return NotFound("User not found.");

            // 1. Vérifier l’ancien email
            if (!string.Equals(user.Email, dto.CurrentEmail, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Current email is incorrect.");

            // 2. Vérifier le mot de passe (même logique que LOGIN)
            if (user.PasswordHash != PasswordHasher.Hash(dto.Password))
                return Unauthorized("Invalid password");

            // 3. Vérifier que le nouvel email n’est pas déjà utilisé
            var exists = await _db.Users.AnyAsync(u => u.Email == dto.NewEmail && u.Id != guid);
            if (exists)
                return BadRequest("This email is already used by another account.");

            // 4. Mise à jour dans Users
            user.Email = dto.NewEmail;

            // 5. Mise à jour dans Students si applicable
            if (user.UserType == "Student")
            {
                var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == guid);
                if (student != null)
                    student.Email = dto.NewEmail;
            }

            await _db.SaveChangesAsync();

            // 6. Forcer la reconnexion
            return Ok(new
            {
                message = "Email updated successfully. Please login again.",
                newEmail = dto.NewEmail
            });
        }




        // ---------------------------------------------------------
        //  UPDATE PHONE /api/Profile/update-phone
        // ---------------------------------------------------------
        [Authorize]
        [HttpPut("update-phone")]
        public async Task<IActionResult> UpdatePhone([FromBody] UpdatePhoneDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Lire l'ID utilisateur (même logique que update-email)
            var userId =
                User.FindFirstValue("sub") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Invalid token.");

            var guid = Guid.Parse(userId);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == guid);
            if (user == null)
                return NotFound("User not found.");

            // 1. Vérifier l'ancien numéro
            if (!string.Equals(user.Phone, dto.CurrentPhone, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Current phone is incorrect.");

            // 2. Vérifier le mot de passe (même logique que LOGIN)
            if (user.PasswordHash != PasswordHasher.Hash(dto.Password))
                return Unauthorized("Invalid password");

            // 3. Vérifier que le nouveau numéro n'existe pas déjà
            var exists = await _db.Users.AnyAsync(u => u.Phone == dto.NewPhone && u.Id != guid);
            if (exists)
                return BadRequest("Phone already exists");

            // 4. Mise à jour dans Users
            user.Phone = dto.NewPhone;

            // 5. Mise à jour dans Students si applicable
            if (user.UserType == "Student")
            {
                var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == guid);
                if (student != null)
                    student.Phone = dto.NewPhone;
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Phone updated successfully",
                phone = dto.NewPhone
            });
        }


        // ---------------------------------------------------------
        //  UPDATE PASSWORD /api/Profile/update-password
        // ---------------------------------------------------------
        [Authorize]
        [HttpPut("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId =
                User.FindFirstValue("sub") ??
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized("Invalid token.");

            var guid = Guid.Parse(userId);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == guid);
            if (user == null)
                return NotFound("User not found.");

            // 1. Vérifier l'ancien mot de passe
            if (user.PasswordHash != PasswordHasher.Hash(dto.CurrentPassword))
                return Unauthorized("Current password is incorrect.");

            // 2. Vérifier que le nouveau mot de passe est différent
            if (dto.CurrentPassword == dto.NewPassword)
                return BadRequest("New password must be different from the current password.");

            // 3. Mettre à jour le mot de passe
            user.PasswordHash = PasswordHasher.Hash(dto.NewPassword);

            // 4. Mise à jour dans Students si applicable
            if (user.UserType == "Student")
            {
                var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == guid);
                if (student != null)
                    student.PasswordHash = PasswordHasher.Hash(dto.NewPassword);
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Password updated successfully. Please login again."
            });
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
                if (student != null) student.AvatarUrl = url;
            }

            await _db.SaveChangesAsync();

            return Ok(new { message = "Avatar updated successfully", avatar = url });
        }
    }
}
