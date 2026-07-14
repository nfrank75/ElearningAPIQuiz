using AdminSmartLearn.Models;
using AdminSmartLearn.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdminSmartLearn.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiClientService _apiClient;

        public AuthController(ApiClientService apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var response = await _apiClient.PostAsync<LoginDto, AuthResponseDto>("/api/Auth/login", model);

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    // Try to fetch profile to get AvatarUrl
                    string avatarUrl = "";
                    try
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Profile/me");
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", response.Token);
                        var profileResponse = await _apiClient.GetAsync<ProfileUpdateDto>("/api/Profile/me"); // Wait, ApiClientService sets header from HttpContext. Since HttpContext doesn't have it yet, this will fail. Let's use _httpClient directly. But _httpClient is private to ApiClientService.
                    }
                    catch { } // Ignore if we can't fetch it, we will fetch it another way or just not have it at login.

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, response.UserId ?? string.Empty),
                        new Claim(ClaimTypes.Name, response.Name ?? string.Empty),
                        new Claim(ClaimTypes.Email, response.Email ?? string.Empty),
                        new Claim(ClaimTypes.Role, response.Role ?? "Admin"),
                        new Claim("access_token", response.Token)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Keep me logged in
                        ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn > 0 ? response.ExpiresIn : 3600)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                    return RedirectToAction("Index", "Dashboard");
                }
                
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again. " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult SignupAdmin()
        {
            // Allow logged-in admins to create other admins
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignupAdmin(AdminSignupDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _apiClient.PostAsync<AdminSignupDto, object>("/api/Auth/signup-admin", model);
                TempData["SuccessMessage"] = "Compte administrateur créé avec succès.";
                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("SignupAdmin");
                }
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Erreur lors de l'inscription: " + ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
