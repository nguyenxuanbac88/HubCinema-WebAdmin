using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using HubCinemaAdmin.Models;
using System.Collections.Generic;

namespace HubCinemaAdmin.Controllers
{
    public class NewsManagementController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "http://api.dvxuanbac.com:2030/api/News";

        public NewsManagementController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ✅ Trang danh sách bài viết (Index)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync(_apiBaseUrl);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Không thể tải danh sách bài viết.";
                return View(new List<News>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var newsList = JsonSerializer.Deserialize<List<News>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(newsList);
        }

        // ✅ Trang tạo bài viết
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(News news)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(news), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiBaseUrl, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Tạo bài viết thất bại!";
            return View(news);
        }
    }
}
