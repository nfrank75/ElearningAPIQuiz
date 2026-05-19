using ElearningAPI.DTOs;
using ElearningAPI.Helpers;
using ElearningAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearningAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // ---------------- SIGNUP STUDENT ----------------
        [HttpPost("signup-student")]
        public async Task<IActionResult> SignupStudent(StudentSignupDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required");

            if (string.IsNullOrWhiteSpace(dto.Email) && string.IsNullOrWhiteSpace(dto.Phone))
                return BadRequest("Email or phone is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Password is required");

            // Vérification email/phone dans Users
            if (!string.IsNullOrWhiteSpace(dto.Email) &&
                await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            if (!string.IsNullOrWhiteSpace(dto.Phone) &&
                await _db.Users.AnyAsync(u => u.Phone == dto.Phone))
                return BadRequest("Phone already exists");

            // Student
            var student = new Student
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Country = dto.Country,
                City = dto.City,
                SchoolName = dto.SchoolName,
                ClassName = dto.ClassName,
                Role = "Student",
                IsMember = false
            };

            _db.Students.Add(student);
            await _db.SaveChangesAsync();

            // User (login + JWT)
            var user = new User
            {
                Id = student.Id,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = student.PasswordHash,
                Role = "Student",
                UserType = "Student",
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Student created successfully",
                userId = student.Id,
                name = dto.Name,
                email = dto.Email,
                phone = dto.Phone,
                userType = "Student",
                role = "Student",
                code = 201
            });
        }

        // ---------------- SIGNUP ADMIN ----------------
        [HttpPost("signup-admin")]
        public async Task<IActionResult> SignupAdmin(AdminSignupDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required");

            if (string.IsNullOrWhiteSpace(dto.Email) && string.IsNullOrWhiteSpace(dto.Phone))
                return BadRequest("Email or phone is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Password is required");

            // Vérification email/phone dans Users
            if (!string.IsNullOrWhiteSpace(dto.Email) &&
                await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            if (!string.IsNullOrWhiteSpace(dto.Phone) &&
                await _db.Users.AnyAsync(u => u.Phone == dto.Phone))
                return BadRequest("Phone already exists");

            // Admin
            var admin = new ElearningAPI.Models.Admin
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Role = "Admin"
            };

            _db.Admins.Add(admin);
            await _db.SaveChangesAsync();

            // User (login + JWT)
            var user = new User
            {
                Id = admin.Id,
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = admin.PasswordHash,
                Role = "Admin",
                UserType = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Admin created successfully",
                userId = admin.Id,
                name = dto.Name,
                email = dto.Email,
                phone = dto.Phone,
                userType = "Admin",
                role = "Admin",
                code = 201
            });
        }


        // ---------------- LOGIN ----------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request");

            if (string.IsNullOrWhiteSpace(dto.Email) && string.IsNullOrWhiteSpace(dto.Phone))
                return BadRequest("email or phone is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Password is required");

            User? user = null;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null && !string.IsNullOrWhiteSpace(dto.Phone))
                user = await _db.Users.FirstOrDefaultAsync(u => u.Phone == dto.Phone);

            if (user == null)
                return Unauthorized("Invalid credentials");

            if (user.PasswordHash != PasswordHasher.Hash(dto.Password))
                return Unauthorized("Invalid password");

            // Génération des tokens
            var accessToken = JwtHelper.GenerateAccessToken(user, _config);
            var refreshToken = JwtHelper.GenerateRefreshToken();

            // Sauvegarder le refresh token
            var refresh = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            _db.RefreshTokens.Add(refresh);
            await _db.SaveChangesAsync();

            // -------------------------
            //   LOGIN STUDENT
            // -------------------------
            if (user.UserType == "Student")
            {
                var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == user.Id);

                if (student == null)
                    return NotFound("Student profile not found");

                return Ok(new
                {
                    message = "Login successful",
                    token = accessToken,
                    refreshToken = refreshToken,
                    role = user.Role,
                    userType = user.UserType,
                    userId = user.Id,
                    expiresIn = 3600,

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
            //   LOGIN ADMIN
            // -------------------------
            return Ok(new
            {
                message = "Login successful",
                token = accessToken,
                refreshToken = refreshToken,
                role = user.Role,
                userType = user.UserType,
                userId = user.Id,
                expiresIn = 3600,

                // Infos Admin
                name = user.Name,
                email = user.Email,
                phone = user.Phone
            });
        }


        // ---------------- REFRESH TOKEN ----------------
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest("Invalid request");

            var stored = await _db.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken && r.RevokedAt == null);

            if (stored == null)
                return Unauthorized("Invalid refresh token");

            if (stored.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("Refresh token expired");

            var user = stored.User;

            var newAccessToken = JwtHelper.GenerateAccessToken(user, _config);
            var newRefreshToken = JwtHelper.GenerateRefreshToken();

            stored.RevokedAt = DateTime.UtcNow;

            var newRefresh = new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            _db.RefreshTokens.Add(newRefresh);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                token = newAccessToken,
                refreshToken = newRefreshToken,
                role = user.Role,
                userType = user.UserType,
                userId = user.Id,
                expiresIn = 3600
            });
        }

        // ---------------- LOGOUT ----------------
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken))
                return BadRequest("Refresh token is required");

            var stored = await _db.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == dto.RefreshToken && r.RevokedAt == null);

            if (stored == null)
                return Unauthorized("Invalid or already revoked refresh token");

            // Invalid the refresh token
            stored.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Logout successful. Token revoked.",
                code = 200
            });
        }


    }
}
