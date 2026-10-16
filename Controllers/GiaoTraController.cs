using Microsoft.AspNetCore.Mvc;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Services;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers
{
    public class GiaoTraController : Controller 
    {
        private readonly GiaoTraService _service;

        public GiaoTraController(GiaoTraService service)
        {
            _service = service;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult KiemTra(GiaoTra giaoTra)
        {
            if (!ModelState.IsValid)
            {
                TempData["Loi"] =
                    "Dữ liệu giao trả chưa hợp lệ.";

                return RedirectToAction(nameof(Index));
            }

            if (!_service.KiemTraDuLieu(
                giaoTra, out string thongBao))
            {
                TempData["Loi"] = thongBao;
                return RedirectToAction(nameof(Index));
            }

            if (!_service.KiemTraNgayGiaoTra(
                giaoTra.NgayGiaoTra, out thongBao))
            {
                TempData["Loi"] = thongBao;
                return RedirectToAction(nameof(Index));
            }

            TempData["ThongBao"] =
                "Thông tin giao trả hợp lệ. " + thongBao;

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
