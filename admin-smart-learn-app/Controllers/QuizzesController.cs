using AdminSmartLearn.Models;
using AdminSmartLearn.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminSmartLearn.Controllers
{
    [Authorize]
    public class QuizzesController : Controller
    {
        private readonly ApiClientService _apiClient;

        public QuizzesController(ApiClientService apiClient)
        {
            _apiClient = apiClient;
        }

        // GET: Quizzes
        public async Task<IActionResult> Index(string levelId = "", string subjectId = "")
        {
            await PopulateViewBags(); // Provide dropdown options to the view

            if (string.IsNullOrEmpty(levelId) || string.IsNullOrEmpty(subjectId))
            {
                ViewBag.PromptSelection = true;
                return View(new List<QuizDto>());
            }

            try
            {
                var url = $"/api/Quiz?levelId={levelId}&subjectId={subjectId}";
                var quizzes = await _apiClient.GetAsync<IEnumerable<QuizDto>>(url);
                ViewBag.SelectedLevelId = levelId;
                ViewBag.SelectedSubjectId = subjectId;
                return View(quizzes ?? new List<QuizDto>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors du chargement des quiz: " + ex.Message;
                return View(new List<QuizDto>());
            }
        }

        // GET: Quizzes/Create
        public async Task<IActionResult> Create()
        {
            await PopulateViewBags();
            return View();
        }

        // POST: Quizzes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuizCreateDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newQuiz = await _apiClient.PostAsync<QuizCreateDto, QuizDto>("/api/admin/quiz/create", model);
                    TempData["SuccessMessage"] = "Quiz créé avec succès. Vous pouvez maintenant y ajouter des questions.";
                    
                    if (newQuiz != null && !string.IsNullOrEmpty(newQuiz.Id))
                    {
                        return RedirectToAction(nameof(Details), new { id = newQuiz.Id, levelId = model.LevelId, subjectId = model.SubjectId });
                    }
                    
                    return RedirectToAction(nameof(Index), new { levelId = model.LevelId, subjectId = model.SubjectId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erreur lors de la création: " + ex.Message);
                }
            }
            await PopulateViewBags();
            return View(model);
        }

        // GET: Quizzes/Details/5
        public async Task<IActionResult> Details(string id, string levelId, string subjectId)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            if (string.IsNullOrEmpty(levelId) || string.IsNullOrEmpty(subjectId))
            {
                TempData["ErrorMessage"] = "Niveau et matière requis pour afficher les détails.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var url = $"/api/Quiz?levelId={levelId}&subjectId={subjectId}";
                var quizzes = await _apiClient.GetAsync<IEnumerable<QuizDto>>(url);
                var quiz = quizzes?.FirstOrDefault(q => q.Id == id);

                if (quiz == null) return NotFound();
                
                ViewBag.LevelId = levelId;
                ViewBag.SubjectId = subjectId;
                return View(quiz);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors du chargement des détails: " + ex.Message;
                return RedirectToAction(nameof(Index), new { levelId, subjectId });
            }
        }


        // POST: Quizzes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, string levelId, string subjectId)
        {
            try
            {
                await _apiClient.DeleteAsync($"/api/admin/quiz/{id}");
                TempData["SuccessMessage"] = "Quiz supprimé.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors de la suppression: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { levelId, subjectId });
        }

        // POST: Quizzes/AddQuestion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestion(string quizId, string levelId, string subjectId, AddQuestionDto model)
        {
            try
            {
                await _apiClient.PostAsync<AddQuestionDto, object>($"/api/admin/quiz/{quizId}/add-question", model);
                TempData["SuccessMessage"] = "Question ajoutée avec succès.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors de l'ajout de la question: " + ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id = quizId, levelId, subjectId });
        }

        // POST: Quizzes/DeleteQuestion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestion(string quizId, string questionId, string levelId, string subjectId)
        {
            try
            {
                await _apiClient.DeleteAsync($"/api/admin/quiz/question/{questionId}");
                TempData["SuccessMessage"] = "Question supprimée.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors de la suppression de la question: " + ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id = quizId, levelId, subjectId });
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
