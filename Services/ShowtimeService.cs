// Services/ShowtimeService.cs
using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Newtonsoft.Json;

namespace HubCinemaAdmin.Services
{
    public class ShowtimeService
    {
        private readonly HttpClient _httpClient;

        public ShowtimeService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
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

        public async Task<List<ShowtimeDTO>> GetTimelineAsync(string ngay, int maRap)
        {
            if (string.IsNullOrWhiteSpace(ngay) || maRap <= 0)
                return new List<ShowtimeDTO>();

            try
            {
                var url = $"{LinkHost.Url}/Schedule/GetTimeline?ngay={ngay}&maRap={maRap}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<List<ShowtimeDTO>>(json);

                return list ?? new List<ShowtimeDTO>();
            }
            catch
            {
                return new List<ShowtimeDTO>();
            }
        }
    }
}
