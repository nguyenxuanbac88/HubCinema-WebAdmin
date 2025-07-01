using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace HubCinemaAdmin.Controllers
{

    public class CinemaManagementController : Controller
    {
        private readonly HttpClient _httpClient;
        public CinemaManagementController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public IActionResult CreateCinema()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCinema(CinemaDTO cinemaDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(cinemaDTO);
            }
            var response = await _httpClient.PostAsJsonAsync(LinkHost.Url + "/AdminPOST/CreateCinema", cinemaDTO);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Tạo rạp chiếu thành công!";
                return RedirectToAction("LoadListCinema");
            }
            else
            {
                TempData["Error"] = "Tạo rạp chiếu thất bại! Vui lòng kiểm tra lại thông tin.";
                return View(cinemaDTO);
            }
        }
        public async Task<IActionResult> LoadListCinema()
        {
            var response = await _httpClient.GetAsync(LinkHost.Url + "/Public/GetCinemas");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var cinemas = JsonConvert.DeserializeObject<List<CinemaDTO>>(jsonString);
                return View(cinemas);
            }
            TempData["Error"] = "Không thể tải danh sách rạp chiếu!";
            return View(new List<CinemaDTO>());
        }
        // GET: EditCinema
        public async Task<IActionResult> EditCinema(int id)
        {
            // Lấy thông tin rạp
            var response = await _httpClient.GetAsync(LinkHost.Url + $"/Public/GetCinemaById/{id}");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("LoadListCinema");

            var cinema = await response.Content.ReadFromJsonAsync<CinemaDTO>();
            if (cinema == null)
                return RedirectToAction("LoadListCinema");

            // Gọi API lấy danh sách phòng theo tên rạp
            var roomResponse = await _httpClient.GetAsync(LinkHost.Url + $"/Admin/GetRoomsByCinemaName/{Uri.EscapeDataString(cinema.CinemaName)}");
            if (roomResponse.IsSuccessStatusCode)
            {
                var rooms = await roomResponse.Content.ReadFromJsonAsync<List<RoomDTO>>();
                ViewBag.Rooms = rooms;
            }
            else
            {
                ViewBag.Rooms = new List<RoomDTO>();
            }

            return View(cinema);
        }


        // POST: EditCinema
        [HttpPost]
        public async Task<IActionResult> EditCinema(CinemaDTO cinemaDTO)
        {
            if (!ModelState.IsValid)
            {
                // Nếu model không hợp lệ, load lại danh sách phòng
                var roomResponse = await _httpClient.GetAsync(LinkHost.Url + $"/Public/GetRoomsByCinemaID/{cinemaDTO.IDCinema}");
                if (roomResponse.IsSuccessStatusCode)
                {
                    var rooms = await roomResponse.Content.ReadFromJsonAsync<List<RoomDTO>>();
                    ViewBag.Rooms = rooms;
                }
                else
                {
                    ViewBag.Rooms = new List<RoomDTO>();
                }

                return View(cinemaDTO);
            }

            // Gọi API cập nhật rạp
            var response = await _httpClient.PutAsJsonAsync(LinkHost.Url + $"/Admin/UpdateCinema/{cinemaDTO.IDCinema}", cinemaDTO);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật rạp chiếu thành công!";
                return RedirectToAction("LoadListCinema");
            }

            // Nếu cập nhật thất bại, load lại danh sách phòng
            var fallbackRoomResponse = await _httpClient.GetAsync(LinkHost.Url + $"/Public/GetRoomsByCinemaID/{cinemaDTO.IDCinema}");
            if (fallbackRoomResponse.IsSuccessStatusCode)
            {
                var rooms = await fallbackRoomResponse.Content.ReadFromJsonAsync<List<RoomDTO>>();
                ViewBag.Rooms = rooms;
            }
            else
            {
                ViewBag.Rooms = new List<RoomDTO>();
            }

            TempData["Error"] = "Cập nhật rạp chiếu thất bại!";
            return View(cinemaDTO);
        }

    }
}
