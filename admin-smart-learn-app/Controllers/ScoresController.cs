using AdminSmartLearn.Models;
using AdminSmartLearn.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminSmartLearn.Controllers
{
    [Authorize]
    public class ScoresController : Controller
    {
        private readonly ApiClientService _apiClient;

        public ScoresController(ApiClientService apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index(string quizId = "")
        {
            try
            {
                IEnumerable<ScoreDto>? scores;
                
                if (string.IsNullOrEmpty(quizId))
                {
                    scores = await _apiClient.GetAsync<IEnumerable<ScoreDto>>("/api/Score/all");
                }
                else
                {
                    scores = await _apiClient.GetAsync<IEnumerable<ScoreDto>>($"/api/Score/quiz/{quizId}");
                    ViewBag.QuizId = quizId;
                }
                
                return View(scores ?? new List<ScoreDto>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Erreur lors du chargement des scores: " + ex.Message;
                return View(new List<ScoreDto>());
            }
        }
    }
}
