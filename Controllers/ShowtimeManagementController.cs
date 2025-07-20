using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HubCinemaAdmin.Controllers
{
    public class ShowtimeManagementController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "http://api.dvxuanbac.com:2030";

        public ShowtimeManagementController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public async Task<IActionResult> Timeline()
        {
            try
            {
                var url = $"{_apiBaseUrl}/api/Public/GetCinemas";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var cinemas = JsonConvert.DeserializeObject<List<CinemaDTO>>(json);

                ViewBag.Cinemas = cinemas;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi lấy danh sách rạp: " + ex.Message);
                ViewBag.Cinemas = new List<CinemaDTO>();
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTimeline(string ngay, int maRap)
        {
            if (string.IsNullOrWhiteSpace(ngay) || maRap <= 0)
            {
                return BadRequest(new { success = false, message = "Thiếu ngày hoặc mã rạp hợp lệ." });
            }

            try
            {
                var url = $"{_apiBaseUrl}/api/Schedule/GetTimeline?ngay={ngay}&maRap={maRap}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonData = await response.Content.ReadAsStringAsync();

                var rawList = JsonConvert.DeserializeObject<List<ShowtimeDTO>>(jsonData);

                if (rawList == null || !rawList.Any())
                    return NotFound(new { success = false, message = "Không có suất chiếu nào." });

                return Json(rawList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi gọi API lịch chiếu." });
            }
        }

    }
}
