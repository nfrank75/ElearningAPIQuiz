using AdminSmartLearn.Models;
using AdminSmartLearn.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminSmartLearn.Controllers
{
    [Authorize]
    public class EpreuvesController : Controller
    {
        private readonly ApiClientService _apiClient;

        public EpreuvesController(ApiClientService apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Let's fetch all epreuves: Uncorrected and Corrected
                // We don't have a single "Get All" endpoint without parameters maybe, but let's try calling uncorrected and corrected without params to see if it returns lists.
                // Assuming `/api/Epreuve/uncorrected` and `/api/Epreuve/corrected` accept empty query params.
                var uncorrected = await _apiClient.GetAsync<IEnumerable<EpreuveDto>>("/api/Epreuve/uncorrected") ?? new List<EpreuveDto>();
                var corrected = await _apiClient.GetAsync<IEnumerable<EpreuveDto>>("/api/Epreuve/corrected") ?? new List<EpreuveDto>();
                
                ViewBag.Uncorrected = uncorrected;
                ViewBag.Corrected = corrected;
                ViewBag.DebugJson = System.Text.Json.JsonSerializer.Serialize(uncorrected.FirstOrDefault());
                
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors du chargement des épreuves: " + ex.Message;
                ViewBag.Uncorrected = new List<EpreuveDto>();
                ViewBag.Corrected = new List<EpreuveDto>();
                return View();
            }
        }

        public async Task<IActionResult> Upload()
        {
            await PopulateViewBags();
            return View(new EpreuveUploadDto { Year = DateTime.Now.Year });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(EpreuveUploadDto model, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "Le fichier est requis.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using var content = new MultipartFormDataContent();
                    
                    if (!string.IsNullOrEmpty(model.Title))
                        content.Add(new StringContent(model.Title), "title");
                    content.Add(new StringContent(model.Year.ToString()), "year");
                    if (!string.IsNullOrEmpty(model.SubjectId))
                        content.Add(new StringContent(model.SubjectId), "subjectId");
                    if (!string.IsNullOrEmpty(model.LevelId))
                        content.Add(new StringContent(model.LevelId), "levelId");
                    content.Add(new StringContent(model.IsCorrected.ToString()), "isCorrected");
                    
                    using var stream = file.OpenReadStream();
                    using var streamContent = new StreamContent(stream);
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                    content.Add(streamContent, "file", file.FileName);

                    await _apiClient.PostMultipartAsync<object>("/api/Epreuve/upload", content);
                    
                    TempData["SuccessMessage"] = "Épreuve uploadée avec succès.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de l'upload: " + ex.Message);
                }
            }

            await PopulateViewBags();
            return View(model);
        }

        private async Task PopulateViewBags()
        {
            try
            {
                var levels = await _apiClient.GetAsync<IEnumerable<LevelDto>>("/api/levels");
                var subjects = await _apiClient.GetAsync<IEnumerable<SubjectDto>>("/api/subjects");
                
                ViewBag.Levels = new SelectList(levels ?? new List<LevelDto>(), "Id", "Name");
                ViewBag.Subjects = new SelectList(subjects ?? new List<SubjectDto>(), "Id", "Name");
            }
            catch
            {
                ViewBag.Levels = new SelectList(new List<LevelDto>());
                ViewBag.Subjects = new SelectList(new List<SubjectDto>());
            }
        }
    }
}
