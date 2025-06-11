using Microsoft.AspNetCore.Mvc;

namespace HubCinemaAdmin.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
