using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;

namespace HubCinemaAdmin.Controllers
{
    public class MovieManagementController : Controller
    {
        private readonly HttpClient _httpClient;
        public MovieManagementController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(MovieDTO movie)
        {
            if (!ModelState.IsValid)
                return View(movie);

            var response = await _httpClient.PostAsJsonAsync("http://localhost:5264/api/Public/CreateMovie", movie);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Tạo phim thành công!";
                return RedirectToAction("LoadListMovie");
            }
            else
            {
                TempData["Error"] = "Tạo phim thất bại!";
                return View(movie);
            }
        }
        public async Task<IActionResult> LoadListMovie()
        {
            var response = await _httpClient.GetAsync("http://localhost:5264/api/Public/GetMovies");
            if (response.IsSuccessStatusCode)
            {
                var movies = await response.Content.ReadFromJsonAsync<List<MovieDTO>>();
                return View(movies);
            }
            else
            {
                TempData["Error"] = "Không thể tải danh sách phim!";
                return View(new List<MovieDTO>());
            }
        }
        public async Task<IActionResult> EditMovie(int id)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5264/api/Public/GetMovieById/{id}");
            if (response.IsSuccessStatusCode)
            {
                var movie = await response.Content.ReadFromJsonAsync<MovieDTO>();
                return View(movie);
            }
            return RedirectToAction("LoadListMovie");
        }
        [HttpPost]
        public async Task<IActionResult> EditMovie(MovieDTO movie)
        {
            if (!ModelState.IsValid)
                return View(movie);
            var response = await _httpClient.PutAsJsonAsync($"http://localhost:5264/api/Public/UpdateMovie/{movie.IDMovie}", movie);
            if(response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật phim thành công!";
                return RedirectToAction("LoadListMovie");
            }
            else
            {
                TempData["Error"] = "Cập nhật phim thất bại!";
                return View(movie);
            }
        }
    }
}
