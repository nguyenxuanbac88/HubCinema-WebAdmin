using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HubCinemaAdmin.Controllers
{
    public class FoodManagementController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public FoodManagementController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult CreateFood()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFood(FoodDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthorizedClient(_httpClientFactory);

            var response = await client.PostAsJsonAsync(LinkHost.Url + "/Admin/CreateFood", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Tạo đồ ăn thành công!";
                return RedirectToAction("LoadListCinema");
            }

            TempData["Error"] = "Tạo đồ ăn thất bại!";
            return View(dto);
        }

    }
}
