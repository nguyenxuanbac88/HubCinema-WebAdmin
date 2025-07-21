// Controllers/ShowtimeManagementController.cs
using HubCinemaAdmin.Models;
using HubCinemaAdmin.Services;
using Microsoft.AspNetCore.Mvc;

namespace HubCinemaAdmin.Controllers
{
    public class ShowtimeManagementController : Controller
    {
        private readonly ShowtimeService _showtimeService;

        public ShowtimeManagementController(ShowtimeService showtimeService)
        {
            _showtimeService = showtimeService;
        }

        public async Task<IActionResult> Timeline()
        {
            var cinemas = await _showtimeService.GetCinemasAsync();
            ViewBag.Cinemas = cinemas;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTimeline(string ngay, int maRap)
        {
            if (string.IsNullOrWhiteSpace(ngay) || maRap <= 0)
                return BadRequest(new { success = false, message = "Thiếu ngày hoặc mã rạp hợp lệ." });

            var data = await _showtimeService.GetTimelineAsync(ngay, maRap);
            if (!data.Any())
                return NotFound(new { success = false, message = "Không có suất chiếu nào." });

            return Json(data);
        }
    }
}
