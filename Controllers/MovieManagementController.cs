using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HubCinemaAdmin.Controllers
{
    public class MovieManagementController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MovieManagementController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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

            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthorizedClient(_httpClientFactory);
            var response = await client.PostAsJsonAsync(LinkHost.Url + "/Admin/CreateMovie", movie);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Tạo phim thành công!";
                return RedirectToAction("LoadListMovie");
            }

            TempData["Error"] = "Tạo phim thất bại!";
            return View(movie);
        }

        public async Task<IActionResult> LoadListMovie(int pageNumber = 1)
        {
            int pageSize = 10;
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(LinkHost.Url + "/Public/GetMovies");
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

            TempData["Error"] = "Không thể tải danh sách phim!";
            return View(new List<MovieDTO>());
        }

        public async Task<IActionResult> EditMovie(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(LinkHost.Url + $"/Public/GetMovieById/{id}");
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

            if (!IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            var client = CreateAuthorizedClient(_httpClientFactory);
            var response = await client.PutAsJsonAsync(LinkHost.Url + $"/Admin/UpdateMovie/{movie.IDMovie}", movie);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật phim thành công!";
                return RedirectToAction("LoadListMovie");
            }

            TempData["Error"] = "Cập nhật phim thất bại!";
            return View(movie);
        }
    }
}
