using Microsoft.AspNetCore.Mvc;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Services;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers
{
    public class PhanCongXuLyController : Controller 
    {
        private readonly PhanCongXuLyService _service;

        public PhanCongXuLyController()
        {
            _service = new PhanCongXuLyService();
        }

        // Trang danh sách sẽ được kết nối database ở bước sau.
        public IActionResult Index()
        {
            return View(new List<PhanCongXuLy>());
        }

        // Kiểm tra dữ liệu trước khi tạo phân công.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult KiemTra(PhanCongXuLy phanCong)
        {
            if (!ModelState.IsValid)
            {
                TempData["Loi"] = "Dữ liệu nhập chưa hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (!_service.KiemTraPhanCong(phanCong, out string thongBao))
            {
                TempData["Loi"] = thongBao;
                return RedirectToAction(nameof(Index));
            }

            TempData["ThongBao"] = thongBao;
            return RedirectToAction(nameof(Index));
        }
    }
}
