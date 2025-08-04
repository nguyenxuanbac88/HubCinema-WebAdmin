// Services/ShowtimeService.cs
using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace HubCinemaAdmin.Services
{
    public class ShowtimeService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ShowtimeService(HttpClient httpClient, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }
        private HttpClient CreateAuthorizedClient()
        {
            var client = _httpClientFactory.CreateClient();
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
        public async Task<List<CinemaDTO>> GetCinemasAsync()
        {
            try
            {
                var url = $"{LinkHost.Url}/Public/GetCinemas";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var cinemas = JsonConvert.DeserializeObject<List<CinemaDTO>>(json);

                return cinemas ?? new List<CinemaDTO>();
            }
            catch
            {
                return new List<CinemaDTO>();
            }
        }
        public async Task<bool> CreateScheduleAsync(ShowtimeDTO showtimeDTO)
        {
            var client = CreateAuthorizedClient();
            var response = await client.PostAsJsonAsync(LinkHost.Url + "/Schedule/CreateSchedule", showtimeDTO);
            var json = System.Text.Json.JsonSerializer.Serialize(showtimeDTO);
            Console.WriteLine("ROOM DTO JSON SENT: " + json);
            return response.IsSuccessStatusCode;
        }
        public async Task<List<ShowtimeTimelineDTO>> GetTimelineAsync(string ngay, int maRap)
        {
            if (string.IsNullOrWhiteSpace(ngay) || maRap <= 0)
                return new List<ShowtimeTimelineDTO>();

            try
            {
                var url = $"{LinkHost.Url}/Schedule/GetTimeline?ngay={ngay}&maRap={maRap}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<ShowtimeTimelineDTO>>(json);

                return list ?? new List<ShowtimeTimelineDTO>();
            }
            catch
            {
                return new List<ShowtimeTimelineDTO>();
            }
        }
    }
}
