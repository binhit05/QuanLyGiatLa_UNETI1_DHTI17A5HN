using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Services;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers
{
    public class PhanCongXuLyController : Controller 
    {
        private readonly AppDbContext _context;
        private readonly PhanCongXuLyService _service;

        public PhanCongXuLyController(AppDbContext context ,PhanCongXuLyService service)
        {
            _context = context;
            _service = service;
        }

        // Trang danh sách sẽ được kết nối database ở bước sau.

        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.PhanCongXuLys
                .AsNoTracking()
                .OrderBy(p => p.MaPhanCong)
                .ToListAsync();

            return View(danhSach);
        }


        // Kiểm tra dữ liệu trước khi tạo phân công.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KiemTra(PhanCongXuLy phanCong)
        {
            // Bước 1: Kiểm tra dữ liệu nhập từ biểu mẫu.
            if (!ModelState.IsValid)
            {
                TempData["Loi"] = "Dữ liệu nhập chưa hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            // Bước 2: Kiểm tra các thông tin cơ bản của phân công.
            if (!_service.KiemTraPhanCong(phanCong, out string thongBao))
            {
                TempData["Loi"] = thongBao;
                return RedirectToAction(nameof(Index));
            }

            // Bước 3: Kiểm tra nhân viên có tồn tại và đang hoạt động.
            var ketQua = await _service.KiemTraNhanVienHoatDongAsync(
                phanCong.MaNhanVien);

            if (!ketQua.HopLe)
            {
                TempData["Loi"] = ketQua.ThongBao;
                return RedirectToAction(nameof(Index));
            }

            // Bước 4: Thông báo kết quả kiểm tra.
            TempData["ThongBao"] =
                "Dữ liệu phân công hợp lệ. " + ketQua.ThongBao;

            return RedirectToAction(nameof(Index));
        }

    }
}
