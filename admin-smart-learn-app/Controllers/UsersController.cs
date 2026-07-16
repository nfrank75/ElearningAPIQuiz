using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AdminSmartLearn.Models;

namespace AdminSmartLearn.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            // For now, since the API doesn't have a route to fetch all users,
            // we will provide some mock data to display the UI as requested by the user.
            // Once the backend API is ready, we will replace this with an ApiClient call,
            // for example: await _apiClient.GetAsync<IEnumerable<UserDto>>("/api/users");

            var users = new List<UserDto>
            {
                new UserDto { Id = "1", Name = "Bertin", Email = "superspipo1@gmail.com", Role = "Admin", Phone = "694048924", IsActive = true, CreatedAt = DateTime.Now.AddDays(-10) },
                new UserDto { Id = "2", Name = "John Doe", Email = "john@example.com", Role = "Utilisateur", Phone = "690000001", IsActive = true, CreatedAt = DateTime.Now.AddDays(-2) },
                new UserDto { Id = "3", Name = "Alice Smith", Email = "alice@example.com", Role = "Utilisateur", Phone = "690000002", IsActive = false, CreatedAt = DateTime.Now.AddDays(-5) }
            };

            return View(users);
        }
    }
}
