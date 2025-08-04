using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using HubCinemaAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HubCinemaAdmin.Controllers
{
    public class ShowtimeManagementController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ShowtimeService _showtimeService;

        public ShowtimeManagementController(IHttpClientFactory httpClientFactory , ShowtimeService showtimeService)
        {
            _httpClientFactory = httpClientFactory;
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
        [HttpPost]
        public async Task<IActionResult> CreateSchedule(ShowtimeDTO showtimeDTO)
        {
            if (!ModelState.IsValid)
                return View(showtimeDTO);

            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");
            var success = await _showtimeService.CreateScheduleAsync(showtimeDTO);

            if (success)
            {
                TempData["Success"] = "Tạo lịch chiếu thành công!";
                return RedirectToAction("Timeline", "ShowtimeManagement");
            }

            TempData["Error"] = "Tạo rạp chiếu thất bại!";
            return View(showtimeDTO);
        }
    }
}
