using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace HubCinemaAdmin.Controllers
{

    public class CinemaManagementController : Controller
    {
        private readonly HttpClient _httpClient;
        private string _linkHost = "http://api.dvxuanbac.com:2030/api";
        public CinemaManagementController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IActionResult> LoadListCinema()
        {
            var response = await _httpClient.GetAsync(_linkHost + "/Public/GetCinemas");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var cinemas = JsonConvert.DeserializeObject<List<CinemaDTO>>(jsonString);
                return View(cinemas);
            }
            TempData["Error"] = "Không thể tải danh sách rạp chiếu!";
            return View(new List<CinemaDTO>());
        }
        public async Task<IActionResult> EditCinema(int id)
        {
            var response = await _httpClient.GetAsync(_linkHost + $"/Public/GetCinemaById/{id}");
            if (response.IsSuccessStatusCode)
            {
                var cinema = await response.Content.ReadFromJsonAsync<CinemaDTO>();
                return View(cinema);
            }
            return RedirectToAction("LoadListCinema");
        }
        [HttpPost]
        public async Task<IActionResult> EditCinema(CinemaDTO cinemaDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(cinemaDTO);
            }

            var response = await _httpClient.PutAsJsonAsync(_linkHost + $"/AdminPUT/UpdateCinema/{cinemaDTO.IDCinema}", cinemaDTO);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật rạp chiếu thành công!";
                return RedirectToAction("LoadListCinema");
            }
            else
            {
                TempData["Error"] = "Cập nhật rạp chiếu thất bại!";
                return View(cinemaDTO);
            }
        }

    }
}
