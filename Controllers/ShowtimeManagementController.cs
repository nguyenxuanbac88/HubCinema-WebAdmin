using Microsoft.AspNetCore.Mvc;

namespace HubCinemaAdmin.Controllers
{
    public class ShowtimeManagementController : Controller
    {
        public IActionResult ShowtimeManagement()
        {
            return View();
        }

    }
}
