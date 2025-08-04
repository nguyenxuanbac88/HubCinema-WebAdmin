using HubCinemaAdmin.Models;
using HubCinemaAdmin.Services;
using Microsoft.AspNetCore.Mvc;

namespace HubCinemaAdmin.Controllers
{
    public class SeatLayoutController : Controller
    {
        private readonly SeatLayoutService _seatLayoutService;

        public SeatLayoutController(SeatLayoutService seatLayoutService)
        {
            _seatLayoutService = seatLayoutService;
        }
        public IActionResult Create(int idRoom, int maRap)
        {
            var model = new CustomSeatLayoutViewModel
            {
                IdRoom = idRoom,
                MaRap = maRap
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Create(CustomSeatLayoutViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var layoutSuccess = await _seatLayoutService.CreateCustomSeatLayoutAsync(model);
            var seatTypesRequest = new SetSeatTypes
            {
                MaPhong = model.IdRoom,
                MaRap = model.MaRap,
                DanhSachGhe = model.SeatTypeRows
            };

            var seatTypeSuccess = await _seatLayoutService.SetSeatTypesAsync(seatTypesRequest);

            bool success = layoutSuccess && seatTypeSuccess;

            TempData[success ? "Success" : "Error"] = success
                ? "Tạo ma trận và cấu hình loại ghế thành công!"
                : "Lỗi khi gửi dữ liệu đến API.";

            return RedirectToAction("EditCinema", "CinemaManagement", new { idCinema = model.MaRap });
        }
    }
}
