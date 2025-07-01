using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing.Printing;

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
            {
                return View(movie);
            }

            var response = await _httpClient.PostAsJsonAsync(LinkHost.Url + "/AdminPOST/CreateMovie", movie);

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
        public async Task<IActionResult> LoadListMovie(int pageNumber = 1)
        {
            int pageSize = 10;
            var response = await _httpClient.GetAsync(LinkHost.Url + "/Public/GetMovies");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var movies = JsonConvert.DeserializeObject<List<MovieDTO>>(jsonString);

                int totalMovies = movies.Count;
                var moviesPaged = movies
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalMovies / pageSize);

                return View(moviesPaged);
            }
            else
            {
                TempData["Error"] = "Không thể tải danh sách phim!";
                return View(new List<MovieDTO>());
            }
        }
        public async Task<IActionResult> EditMovie(int id)
        {
            var response = await _httpClient.GetAsync(LinkHost.Url + $"/Public/GetMovieById/{id}");
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
            {
                return View(movie);
            }

            var response = await _httpClient.PutAsJsonAsync(LinkHost.Url + $"AdminPUT/UpdateMovie/{movie.IDMovie}", movie);
            if (response.IsSuccessStatusCode)
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
