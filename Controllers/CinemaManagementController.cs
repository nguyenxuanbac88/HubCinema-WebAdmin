using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;

namespace HubCinemaAdmin.Controllers
{
    public class CinemaManagementController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CinemaManagementController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult CreateCinema()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCinema(CinemaDTO cinemaDTO)
        {
            if (!ModelState.IsValid)
                return View(cinemaDTO);

            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthorizedClient(_httpClientFactory);

            var response = await client.PostAsJsonAsync(LinkHost.Url + "/Admin/CreateCinema", cinemaDTO);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Tạo rạp chiếu thành công!";
                return RedirectToAction("LoadListCinema");
            }

            TempData["Error"] = "Tạo rạp chiếu thất bại! Vui lòng kiểm tra lại thông tin.";
            return View(cinemaDTO);
        }

        public async Task<IActionResult> LoadListCinema()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(LinkHost.Url + "/Public/GetCinemas");
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
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(LinkHost.Url + $"/Public/GetCinemaById/{id}");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("LoadListCinema");

            var cinema = await response.Content.ReadFromJsonAsync<CinemaDTO>();
            if (cinema == null)
                return RedirectToAction("LoadListCinema");

            var roomResponse = await client.GetAsync(LinkHost.Url + $"/Admin/GetRoomsByCinemaName/{Uri.EscapeDataString(cinema.CinemaName)}");
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

        [HttpPost]
        public async Task<IActionResult> EditCinema(CinemaDTO cinemaDTO)
        {
            if (!ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient();
                var roomResponse = await client.GetAsync(LinkHost.Url + $"/Public/GetRoomsByCinemaID/{cinemaDTO.IDCinema}");
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

            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var authorizedClient = CreateAuthorizedClient(_httpClientFactory);
            var response = await authorizedClient.PutAsJsonAsync(LinkHost.Url + $"/Admin/UpdateCinema/{cinemaDTO.IDCinema}", cinemaDTO);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật rạp chiếu thành công!";
                return RedirectToAction("LoadListCinema");
            }

            var fallbackRoomResponse = await authorizedClient.GetAsync(LinkHost.Url + $"/Public/GetRoomsByCinemaID/{cinemaDTO.IDCinema}");
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
