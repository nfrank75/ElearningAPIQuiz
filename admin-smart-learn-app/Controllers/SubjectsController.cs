using AdminSmartLearn.Models;
using AdminSmartLearn.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminSmartLearn.Controllers
{
    [Authorize]
    public class SubjectsController : Controller
    {
        private readonly ApiClientService _apiClient;

        public SubjectsController(ApiClientService apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: Subjects
        public async Task<IActionResult> Index()
        {
            try
            {
                var subjects = await _apiClient.GetAsync<IEnumerable<SubjectDto>>("/api/subjects");
                return View(subjects ?? new List<SubjectDto>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors du chargement des matières: " + ex.Message;
                return View(new List<SubjectDto>());
            }
        }

        // GET: Subjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subjects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSubjectDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _apiClient.PostAsync<CreateSubjectDto, object>("/api/admin/subjects", model);
                    TempData["SuccessMessage"] = "Matière créée avec succès.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de la création: " + ex.Message);
                }
            }
            return View(model);
        }

        // GET: Subjects/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            try
            {
                var subject = await _apiClient.GetAsync<SubjectDto>($"/api/subjects/{id}");
                if (subject == null) return NotFound();

                var model = new UpdateSubjectDto
                {
                    Name = subject.Name,
                    Description = subject.Description
                };
                return View(model);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: Subjects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateSubjectDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _apiClient.PutAsync<UpdateSubjectDto, object>($"/api/admin/subjects/{id}", model);
                    TempData["SuccessMessage"] = "Matière mise à jour avec succès.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de la mise à jour: " + ex.Message);
                }
            }
            return View(model);
        }

        // POST: Subjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _apiClient.DeleteAsync($"/api/admin/subjects/{id}");
                TempData["SuccessMessage"] = "Matière supprimée avec succès.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors de la suppression: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
