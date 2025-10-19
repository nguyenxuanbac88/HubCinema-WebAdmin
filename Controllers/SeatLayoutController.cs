using HubCinemaAdmin.Models;
using HubCinemaAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HubCinemaAdmin.Controllers
{
    public class SeatLayoutController : Controller
    {
        private readonly SeatLayoutService _seatLayoutService;

        public SeatLayoutController(SeatLayoutService seatLayoutService)
        {
            _seatLayoutService = seatLayoutService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int idRoom, int maRap, string? idLayout = null)
        {
            var seatLayoutResponse = await _seatLayoutService.GetSeatLayoutResponseAsync(idRoom);
            var model = new CustomSeatLayoutViewModel
            {
                IdRoom = idRoom,
                MaRap = maRap
            };

            // Kiểm tra Layout đúng cách
            if (seatLayoutResponse == null || seatLayoutResponse.Layout == null || !seatLayoutResponse.Layout.Any())
            {
                ViewBag.Message = "Phòng chiếu chưa có ma trận ghế. Vui lòng tạo mới.";
                ViewBag.HasExistingLayout = false;
                model.HasExistingLayout = false;
                return View(model);
            }

            // ✅ FIX 1: Bỏ IdLayout vì không tồn tại
            ViewBag.HasExistingLayout = true;
            ViewBag.LayoutId = idLayout; // Chỉ dùng parameter
            
            // Chuyển Layout matrix thành JSON string để hiển thị
            ViewBag.LayoutMatrix = JsonConvert.SerializeObject(seatLayoutResponse.Layout);
            
            model.HasExistingLayout = true;
            
            // ✅ FIX 2: Không assign trực tiếp vào ParsedLayout (read-only)
            // Thay vào đó, truyền qua ViewBag để View tự xử lý
            ViewBag.ExistingLayoutJson = JsonConvert.SerializeObject(seatLayoutResponse.Layout);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomSeatLayoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var layoutSuccess = await _seatLayoutService.CreateCustomSeatLayoutAsync(model);
            
            // Kiểm tra SeatTypeRows có null không
            bool seatTypeSuccess = true;
            if (model.SeatTypeRows != null && model.SeatTypeRows.Any())
            {
                var seatTypesRequest = new SetSeatTypes
                {
                    MaPhong = model.IdRoom,
                    MaRap = model.MaRap,
                    DanhSachGhe = model.SeatTypeRows
                };

                seatTypeSuccess = await _seatLayoutService.SetSeatTypesAsync(seatTypesRequest);
            }

            bool success = layoutSuccess && seatTypeSuccess;

            TempData[success ? "Success" : "Error"] = success
                ? "Tạo ma trận và cấu hình loại ghế thành công!"
                : "Lỗi khi gửi dữ liệu đến API.";

            return RedirectToAction("EditCinema", "CinemaManagement", new { idCinema = model.MaRap });
        }
    }
}
