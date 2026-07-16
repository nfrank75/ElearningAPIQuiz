using AdminSmartLearn.Models;
using AdminSmartLearn.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminSmartLearn.Controllers
{
    [Authorize]
    public class LevelsController : Controller
    {
        private readonly ApiClientService _apiClient;

        public LevelsController(ApiClientService apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: Levels
        public async Task<IActionResult> Index()
        {
            try
            {
                var levels = await _apiClient.GetAsync<IEnumerable<LevelDto>>("/api/levels");
                return View(levels ?? new List<LevelDto>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors du chargement des niveaux: " + ex.Message;
                return View(new List<LevelDto>());
            }
        }

        // GET: Levels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Levels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLevelDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _apiClient.PostAsync<CreateLevelDto, object>("/api/admin/levels", model);
                    TempData["SuccessMessage"] = "Niveau créé avec succès.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de la création: " + ex.Message);
                }
            }
            return View(model);
        }

        // GET: Levels/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            try
            {
                var level = await _apiClient.GetAsync<LevelDto>($"/api/levels/{id}");
                if (level == null) return NotFound();

                var model = new UpdateLevelDto
                {
                    Name = level.Name,
                    Description = level.Description,
                    Order = level.Order
                };
                return View(model);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: Levels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateLevelDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _apiClient.PutAsync<UpdateLevelDto, object>($"/api/admin/levels/{id}", model);
                    TempData["SuccessMessage"] = "Niveau mis à jour avec succès.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de la mise à jour: " + ex.Message);
                }
            }
            return View(model);
        }

        // POST: Levels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await _apiClient.DeleteAsync($"/api/admin/levels/{id}");
                TempData["SuccessMessage"] = "Niveau supprimé avec succès.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors de la suppression: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
