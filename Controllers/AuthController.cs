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

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        // ---------------- SIGNUP STUDENT ----------------
        [HttpPost("signup-student")]
        public async Task<IActionResult> SignupStudent(StudentSignupDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request");

            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Name is required");

            // Email OU Phone obligatoire
            if (string.IsNullOrWhiteSpace(dto.Email) && string.IsNullOrWhiteSpace(dto.Phone))
                return BadRequest("Email or phone is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Password is required");

            // Vérifier email si fourni
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                    return BadRequest("Email already exists");
            }

            // Vérifier phone si fourni
            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                if (await _db.Users.AnyAsync(u => u.Phone == dto.Phone))
                    return BadRequest("Phone already exists");
            }

           

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

            return Ok(new
            {
                message = "Student created successfully",
                userType = "Student",
                role = "Student",
                userId = student.Id,
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

            // Email OU Phone obligatoire
            if (string.IsNullOrWhiteSpace(dto.Email) && string.IsNullOrWhiteSpace(dto.Phone))
                return BadRequest("Email or phone is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Password is required");

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                    return BadRequest("Email already exists");
            }

            if (!string.IsNullOrWhiteSpace(dto.Phone))
            {
                if (await _db.Users.AnyAsync(u => u.Phone == dto.Phone))
                    return BadRequest("Phone already exists");
            }

            var admin = new Admin
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                PasswordHash = PasswordHasher.Hash(dto.Password),
                Role = "Admin"
            };

            _db.Admins.Add(admin);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Admin created successfully",
                userType = "Admin",
                role = "Admin",
                userId = admin.Id,
                code = 201
            });
        }

        // ---------------- LOGIN ----------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request");

            // Email OU Phone obligatoire
            if (
                string.IsNullOrWhiteSpace(dto.Email) &&
                string.IsNullOrWhiteSpace(dto.Phone))
                return BadRequest("email or phone is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Password is required");

            User? user = null;



            // 2. Login via email
            if (user == null && !string.IsNullOrWhiteSpace(dto.Email))
                user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            // 3. Login via phone
            if (user == null && !string.IsNullOrWhiteSpace(dto.Phone))
                user = await _db.Users.FirstOrDefaultAsync(u => u.Phone == dto.Phone);

            if (user == null)
                return Unauthorized("Invalid credentials");

            if (user.PasswordHash != PasswordHasher.Hash(dto.Password))
                return Unauthorized("Invalid password");

            return Ok(new
            {
                message = "Login successful",
                userType = user.GetType().Name,
                userId = user.Id,
                code = 200
            });
        }
    }
}