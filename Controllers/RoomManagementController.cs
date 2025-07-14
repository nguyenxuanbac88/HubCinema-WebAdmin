using HubCinemaAdmin.Helpers;
using HubCinemaAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HubCinemaAdmin.Controllers
{
    public class RoomManagementController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RoomManagementController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<List<RoomDTO>> GetRoomsByCinemaId(int cinemaId)
        {
            var client = CreateAuthorizedClient(_httpClientFactory);

            var response = await client.GetAsync($"{LinkHost.Url}/Admin/GetRoomsByCinemaId/{cinemaId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var rooms = JsonConvert.DeserializeObject<List<RoomDTO>>(json);
                return rooms ?? new List<RoomDTO>();
            }

            return new List<RoomDTO>();
        }
    }
}
