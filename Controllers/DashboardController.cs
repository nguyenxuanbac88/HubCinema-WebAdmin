using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HubCinemaAdmin.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;

        public DashboardController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Dashboard()
        {
            
            string token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                //Test thử 
                token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiMDE5MjMxMjU3MiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiaWF0IjoxNzUyMzM3MTM3LCJuYmYiOjE3NTIzMzcxMzcsImV4cCI6MTc1MjM0MDczNywiaXNzIjoiSFVCQ2luZW1hQVBJIiwiYXVkIjoibXlDaW5lbWFDbGllbnQifQ.dEUVWr_WZurokytD3qodqfW7npybXd-CdahnFD67LXs"; 
            }

            //Gán token vào Header
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //Gọi API
            var response = await _httpClient.GetAsync(LinkHost.DashboardSummary);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ApiError = "Không thể lấy dữ liệu dashboard từ API.";
                return View(new AdminDashboardViewModel());
            }

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<AdminDashboardViewModel>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(model);
        }
    }
}
