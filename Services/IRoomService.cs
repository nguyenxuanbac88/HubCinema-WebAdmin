using HubCinemaAdmin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HubCinemaAdmin.Services
{
    public interface IRoomService
    {
        Task<List<RoomDTO>> GetRoomsByCinemaIdAsync(int cinemaId);
    }
}
