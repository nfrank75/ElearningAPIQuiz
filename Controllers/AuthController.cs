using ElearningAPI.DTOs;
using ElearningAPI.Helpers;
using ElearningAPI.Models;
using ElearningAPI.Services;
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
        private readonly IEmailService _emailService;
        private readonly int _otpExpirationMinutes;

        public AuthController(AppDbContext db, IConfiguration config, IEmailService emailService)
        {
            _db = db;
            _config = config;
            _emailService = emailService;
            _otpExpirationMinutes = _config.GetValue<int>("OtpSettings:ExpirationMinutes");
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


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            // Récupérer l'utilisateur
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            // Réponse générique (sécurité OWASP)
            if (user == null)
                return Ok(new { message = "If an account exists, an OTP has been sent." });

            // Vérifier que l'email de l'utilisateur est valide
            if (string.IsNullOrWhiteSpace(user.Email))
                return BadRequest("User email is invalid.");

            // Générer OTP (6 chiffres)
            var otp = new Random().Next(100000, 999999).ToString();

            var otpHash = PasswordHasher.Hash(otp);

            var resetOtp = new PasswordResetOtp
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                OtpHash = otpHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_otpExpirationMinutes),
                Used = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.PasswordResetOtps.Add(resetOtp);
            await _db.SaveChangesAsync();

            var body = $@"
            <p>Bonjour,</p>
            <p>Votre code OTP pour réinitialiser votre mot de passe est :</p>
            <h2>{otp}</h2>
            <p>Ce code expire dans <strong>10 minutes</strong>.</p>
            <p>Si vous n'êtes pas à l'origine de cette demande, ignorez cet email.</p>
            <p>Cordialement,</p>
            <p>Smart Learn App</p>
            ";  

            await _emailService.SendAsync(user.Email, "Password Reset OTP", body);

            return Ok(new { message = "If an account exists, an OTP has been sent." });
        }


        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Otp))
                return BadRequest("OTP is required.");

            var otpEntry = await _db.PasswordResetOtps
                .FirstOrDefaultAsync(o => o.OtpHash == PasswordHasher.Hash(dto.Otp));

            if (otpEntry == null)
                return BadRequest("Invalid OTP.");

            var now = DateTime.UtcNow;

            // Vérifier expiration
            if (otpEntry.ExpiresAt < now)
                return BadRequest($"OTP expired. Validity was {_otpExpirationMinutes} minutes.");

            if (otpEntry.Used)
                return BadRequest("OTP already used.");

            // Calcul du temps restant
            var remaining = otpEntry.ExpiresAt - now;
            var remainingSeconds = (int)remaining.TotalSeconds;

            return Ok(new
            {
                message = "OTP valid. You may now reset your password.",
                expiresInSeconds = remainingSeconds,
                expiresInMinutes = remainingSeconds / 60
            });
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Otp))
                return BadRequest("OTP is required.");

            if (string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest("New password is required.");

            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            var otpEntry = await _db.PasswordResetOtps
                .FirstOrDefaultAsync(o => o.OtpHash == PasswordHasher.Hash(dto.Otp));

            if (otpEntry == null)
                return BadRequest("Invalid OTP.");

            var now = DateTime.UtcNow;

            // Vérifier expiration
            if (otpEntry.ExpiresAt < now)
                return BadRequest("OTP expired. Please request a new code.");

            // Vérifier si déjà utilisé
            if (otpEntry.Used)
                return BadRequest("OTP already used.");

            // Récupérer l'utilisateur
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == otpEntry.UserId);

            if (user == null)
                return BadRequest("User not found.");

            // Mettre à jour le mot de passe
            user.PasswordHash = PasswordHasher.Hash(dto.NewPassword);

            // Invalider l’OTP
            otpEntry.Used = true;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Password reset successful." });
        }


        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            // Réponse générique (sécurité OWASP)
            if (user == null)
                return Ok(new { message = "If an account exists, a new OTP has been sent." });

            // Générer un nouveau OTP
            var otp = new Random().Next(100000, 999999).ToString();
            var otpHash = PasswordHasher.Hash(otp);

            // Charger la durée depuis appsettings.json
            var expirationMinutes = _config.GetValue<int>("OtpSettings:ExpirationMinutes");

            var resetOtp = new PasswordResetOtp
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                OtpHash = otpHash,
                ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                Used = false,
                CreatedAt = DateTime.UtcNow
            };

            _db.PasswordResetOtps.Add(resetOtp);
            await _db.SaveChangesAsync();

            var body = $@"
            <p>Bonjour,</p>
            <p>Votre nouveau code OTP est :</p>
            <h2>{otp}</h2>
            <p>Ce code expire dans <strong>{expirationMinutes} minutes</strong>.</p>
            ";

            await _emailService.SendAsync(user.Email, "New Password Reset OTP", body);

            return Ok(new { message = "If an account exists, a new OTP has been sent." });
        }





    }
}
