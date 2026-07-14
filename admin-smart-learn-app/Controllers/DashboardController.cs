using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminSmartLearn.Models;
using AdminSmartLearn.Services;

namespace AdminSmartLearn.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApiClientService _apiClient;

        public DashboardController(ApiClientService apiClient)
        {
            _apiClient = apiClient;
        }
        public async Task<IActionResult> Index()
        {
            int levelsCount = 0;
            int subjectsCount = 0;
            int quizzesCount = 0;
            int scoresCount = 0;

            try
            {
                var levels = await _apiClient.GetAsync<IEnumerable<LevelDto>>("/api/levels");
                levelsCount = levels?.Count() ?? 0;
                
                var subjects = await _apiClient.GetAsync<IEnumerable<SubjectDto>>("/api/subjects");
                subjectsCount = subjects?.Count() ?? 0;
                
                var quizzes = await _apiClient.GetAsync<IEnumerable<QuizDto>>("/api/Quiz");
                quizzesCount = quizzes?.Count() ?? 0;

                var scores = await _apiClient.GetAsync<IEnumerable<ScoreDto>>("/api/Score/all");
                scoresCount = scores?.Count() ?? 0;
            }
            catch
            {
                // Ignore errors for stats, they will just show 0
            }

            ViewBag.LevelsCount = levelsCount;
            ViewBag.SubjectsCount = subjectsCount;
            ViewBag.QuizzesCount = quizzesCount;
            ViewBag.ScoresCount = scoresCount;

            return View();
        }
    }
}
