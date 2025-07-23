using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HubCinemaAdmin.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public InvoiceController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            var apiUrl = "http://api.dvxuanbac.com:2030/api/Invoice/all";

            var response = await client.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể lấy dữ liệu hóa đơn từ API.";
                return View(new List<InvoiceAdminViewModel>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var invoices = JsonConvert.DeserializeObject<List<InvoiceAdminViewModel>>(json);
            return View(invoices);
        }
    }
}
