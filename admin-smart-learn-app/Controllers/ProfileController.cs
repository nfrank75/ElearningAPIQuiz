using AdminSmartLearn.Models;
using AdminSmartLearn.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminSmartLearn.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApiClientService _apiClient;

        public ProfileController(ApiClientService apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: Profile/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var profile = await _apiClient.GetAsync<ProfileUpdateDto>("/api/Profile/me");
                return View(profile ?? new ProfileUpdateDto());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur de chargement du profil: " + ex.Message;
                return View(new ProfileUpdateDto());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInfo(ProfileUpdateDto model)
        {
            try
            {
                await _apiClient.PutAsync<ProfileUpdateDto, object>("/api/Profile/update", model);
                TempData["SuccessMessage"] = "Profil mis à jour avec succès.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmail(UpdateEmailDto model)
        {
            try
            {
                await _apiClient.PutAsync<UpdateEmailDto, object>("/api/Profile/update-email", model);
                TempData["SuccessMessage"] = "Email mis à jour avec succès.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePhone(UpdatePhoneDto model)
        {
            try
            {
                await _apiClient.PutAsync<UpdatePhoneDto, object>("/api/Profile/update-phone", model);
                TempData["SuccessMessage"] = "Téléphone mis à jour avec succès.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAvatar(IFormFile avatarFile)
        {
            if (avatarFile == null || avatarFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Veuillez sélectionner un fichier.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var content = new MultipartFormDataContent();
                using var stream = avatarFile.OpenReadStream();
                using var streamContent = new StreamContent(stream);
                streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(avatarFile.ContentType);
                content.Add(streamContent, "file", avatarFile.FileName);

                await _apiClient.PostMultipartAsync<object>("/api/Profile/avatar", content);
                TempData["SuccessMessage"] = "Avatar mis à jour avec succès.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors de l'upload: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
