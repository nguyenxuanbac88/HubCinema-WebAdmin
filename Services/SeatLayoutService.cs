using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HubCinemaAdmin.Services
{
    public class SeatLayoutService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SeatLayoutService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> CreateCustomSeatLayoutAsync(CustomSeatLayoutViewModel vm)
        {
            var apiUrl = $"{LinkHost.Url}/Seat/Custom_Seat_Layout";

            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return false;

            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                IdRoom = vm.IdRoom,
                Layout = vm.ParsedLayout
            };
            request.Content = JsonContent.Create(payload);

            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> SetSeatTypesAsync(SetSeatTypes request)
        {
            var apiUrl = $"{LinkHost.Url}/Seat/set-seat-types";
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return false;

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpRequest.Content = JsonContent.Create(request);

            var response = await _httpClient.SendAsync(httpRequest);
            return response.IsSuccessStatusCode;
        }

    }
}
