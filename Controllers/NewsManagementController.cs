using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using HubCinemaAdmin.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        //Trang danh sách bài viết (Index)
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

        //Trang tạo bài viết
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categoryResponse = await _httpClient.GetAsync("http://api.dvxuanbac.com:2030/api/News/categories");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var json = await categoryResponse.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<Category>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                ViewBag.Categories = categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();
            }
            else
            {
                ViewBag.Categories = new List<SelectListItem>();
                ViewBag.Error = "Không thể tải danh mục!";
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] NewsCreateDTO dto)
        {

            string? thumbnailPath = null;

            //Upload ảnh nếu có
            if (dto.ThumbnailFile != null && dto.ThumbnailFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ThumbnailFile.FileName);
                var saveFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "news");
                Directory.CreateDirectory(saveFolder);
                var savePath = Path.Combine(saveFolder, fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await dto.ThumbnailFile.CopyToAsync(stream);
                }

                //Tạo đường dẫn tuyệt đối từ Request
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                thumbnailPath = $"{baseUrl}/uploads/news/{fileName}";
            }
            else if (!string.IsNullOrWhiteSpace(dto.ThumbnailUrl))
            {
                //Dùng link người dùng nhập nếu không upload
                thumbnailPath = dto.ThumbnailUrl;
            }

            //Chuẩn bị dữ liệu gửi API
            var news = new News
            {
                Title = dto.Title,
                Subtitle = dto.Subtitle,
                Slug = dto.Slug,
                Content = dto.Content,
                Status = dto.Status ?? "A",
                Category = dto.CategoryId ?? 1,
                Thumbnail = thumbnailPath
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(news), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiBaseUrl, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            //Nếu lỗi: hiện thông báo + nạp lại danh mục
            await LoadCategoriesAsync();
            ViewBag.Error = "Tạo bài viết thất bại!";
            return View(dto);
        }

        private async Task LoadCategoriesAsync()
        {
            var categoryResponse = await _httpClient.GetAsync("http://api.dvxuanbac.com:2030/api/News/categories");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var json = await categoryResponse.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<Category>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                ViewBag.Categories = categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    }).ToList();
            }
            else
            {
                ViewBag.Categories = new List<SelectListItem>();
            }
        }

    }
}
