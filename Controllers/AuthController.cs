using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;

namespace HubCinemaAdmin.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;

        public AuthController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri("http://api.dvxuanbac.com:2030/");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", login);
            if (response.IsSuccessStatusCode)
            {
                string token = await response.Content.ReadAsStringAsync();

                HttpContext.Session.SetString("Token", token);
                return RedirectToAction("Dashboard", "Dashboard");
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return View(login);
        }
    }
}
