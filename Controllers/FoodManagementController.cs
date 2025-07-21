using HubCinemaAdmin.Models;
using HubCinemaAdmin.Services;
using Microsoft.AspNetCore.Mvc;

namespace HubCinemaAdmin.Controllers
{
    public class FoodManagementController : BaseController
    {
        private readonly FoodService _foodService;

        public FoodManagementController(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
        {
            _foodService = new FoodService(httpClientFactory, contextAccessor);
        }

        public async Task<IActionResult> CreateFood()
        {
            ViewBag.Cinemas = await _foodService.GetCinemasAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFood(FoodDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cinemas = await _foodService.GetCinemasAsync();
                return View(dto);
            }

            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var createdFood = await _foodService.CreateFoodAsync(dto);
            if (createdFood == null)
            {
                TempData["Error"] = "Tạo món ăn thất bại!";
                ViewBag.Cinemas = await _foodService.GetCinemasAsync();
                return View(dto);
            }

            if (dto.ApplyToAllCinemas && createdFood.IDFood != null)
            {
                var allCinemas = await _foodService.GetCinemasAsync();
                dto.SelectedCinemaIds = allCinemas.Select(c => c.IDCinema).ToList();
            }

            if (dto.SelectedCinemaIds?.Any() == true && createdFood.IDFood != null)
            {
                var success = await _foodService.CreateComboForCinemasAsync(
                    createdFood.IDFood.Value, dto.SelectedCinemaIds
                );

                if (!success)
                    TempData["Warning"] = "Tạo món ăn thành công nhưng không thể áp dụng cho các rạp!";
            }

            TempData["Success"] = "Tạo món ăn thành công!";
            return RedirectToAction("LoadListCombo");
        }

        public async Task<IActionResult> LoadListCombo()
        {
            ViewBag.Cinemas = await _foodService.GetCinemasAsync();
            return View();
        }

        public async Task<IActionResult> GetFoodsByCinema(int cinemaId)
        {
            var foods = await _foodService.GetFoodsByCinemaAsync(cinemaId);
            return PartialView("_FoodListPartial", foods);
        }

        public async Task<IActionResult> GetAllCombos()
        {
            var foods = await _foodService.GetAllFoodsAsync();
            return PartialView("_FoodListPartial", foods);
        }
    }
}
