using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
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

        public async Task<IActionResult> CreateFood()
        {
            await LoadCinemasToViewBag();
            return View();
        }

        private async Task LoadCinemasToViewBag()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(LinkHost.Url + "/Public/GetCinemas");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                ViewBag.Cinemas = JsonConvert.DeserializeObject<List<CinemaDTO>>(json);
            }
            else
            {
                ViewBag.Cinemas = new List<CinemaDTO>();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFood(FoodDTO dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCinemasToViewBag();
                return View(dto);
            }

            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthorizedClient(_httpClientFactory);

            var response = await client.PostAsJsonAsync(LinkHost.Url + "/Admin/CreateFood", dto);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Tạo món ăn thất bại!";
                await LoadCinemasToViewBag();
                return View(dto);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var foodCreated = JsonConvert.DeserializeObject<FoodDTO>(responseContent);

            if (dto.ApplyToAllCinemas && foodCreated?.IDFood != null)
            {
                var getAllCinemasClient = _httpClientFactory.CreateClient();
                var getCinemasResp = await getAllCinemasClient.GetAsync(LinkHost.Url + "/Public/GetCinemas");

                if (getCinemasResp.IsSuccessStatusCode)
                {
                    var cinemaJson = await getCinemasResp.Content.ReadAsStringAsync();
                    var allCinemas = JsonConvert.DeserializeObject<List<CinemaDTO>>(cinemaJson);
                    dto.SelectedCinemaIds = allCinemas.Select(c => c.IDCinema).ToList();
                }
            }

            if (dto.SelectedCinemaIds != null && dto.SelectedCinemaIds.Any() && foodCreated?.IDFood != null)
            {
                var comboDto = new CreateComboCinema
                {
                    IdFood = foodCreated.IDFood.Value,
                    IdCinemapList = dto.SelectedCinemaIds
                };

                var json = JsonConvert.SerializeObject(comboDto);
                var comboContent = new StringContent(json, Encoding.UTF8, "application/json");

                var comboResponse = await client.PostAsync(LinkHost.Url + "/Admin/CreateComboForCinemas", comboContent);

                if (!comboResponse.IsSuccessStatusCode)
                {
                    TempData["Warning"] = "Tạo món ăn thành công nhưng không thể áp dụng cho các rạp!";
                }
            }

            TempData["Success"] = "Tạo món ăn thành công!";
            return RedirectToAction("LoadListCombo");
        }

        public async Task<IActionResult> LoadListCombo()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(LinkHost.Url + "/Public/GetCinemas");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var cinemas = JsonConvert.DeserializeObject<List<CinemaDTO>>(json);
                ViewBag.Cinemas = cinemas;
            }
            else
            {
                ViewBag.Cinemas = new List<CinemaDTO>();
            }

            return View();
        }

        public async Task<IActionResult> GetFoodsByCinema(int cinemaId)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(LinkHost.Url + $"/Public/GetCombosByCinema/{cinemaId}");

            if (!response.IsSuccessStatusCode)
            {
                return Content("<div class='alert alert-danger'>Không thể tải dữ liệu món ăn.</div>", "text/html");
            }

            var content = await response.Content.ReadAsStringAsync();
            var foods = JsonConvert.DeserializeObject<List<FoodDTO>>(content);

            return PartialView("_FoodListPartial", foods);
        }

        public async Task<IActionResult> GetAllCombos()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(LinkHost.Url + "/Public/GetFoods");

            if (!response.IsSuccessStatusCode)
            {
                return Content("<div class='alert alert-danger'>Không thể tải danh sách món ăn.</div>", "text/html");
            }

            var content = await response.Content.ReadAsStringAsync();
            var foods = JsonConvert.DeserializeObject<List<FoodDTO>>(content);

            return PartialView("_FoodListPartial", foods);
        }
    }
}
