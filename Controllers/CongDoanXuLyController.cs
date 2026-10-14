// Họ và tên: Lê Hoàng Anh
// Mã sinh viên: 23103100214
// Nội dung thực hiện: Tạo CongDoanXuLyController: hiển thị và kết nối với Service

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Services;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers
{
    public class CongDoanXuLyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CongDoanXuLyService _congDoanService;

        public CongDoanXuLyController(AppDbContext context, CongDoanXuLyService congDoanService)
        {
            _context = context;
            _congDoanService = congDoanService;
        }

        //Hiển thị danh sách công đoạn 
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.CongDoanXuLys
                .AsNoTracking()
                .OrderBy(c => c.MaCongDoan)
                .ToArrayAsync();

            return View(danhSach);
        }

        //Bắt đầu công đoạn 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatDau(int id)
        {
            var congDoan = await _context.CongDoanXuLys.FindAsync(id);
            if(congDoan == null)
            {
                return NotFound();
            }
            if(!_congDoanService.BatDau(congDoan, out string thongBao))
            {
                TempData["Loi"] = thongBao;
                return RedirectToAction(nameof(Index));
            }

            await _context.SaveChangesAsync();

            TempData["ThongBao"] = thongBao;
            return RedirectToAction(nameof(Index));
        }

        //Hoàn thành công đoạn 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HoanThanh(int id)
        {
            var congDoan = await _context.CongDoanXuLys.FindAsync(id);
            if(congDoan == null)
            {
                return NotFound();
            }
            if (!_congDoanService.HoanThanh(congDoan, out string thongBao))
            {
                TempData["Loi"] = thongBao;
                return RedirectToAction(nameof(Index));
            }

            await _context.SaveChangesAsync();

            TempData["ThongBao"] = thongBao;
            return RedirectToAction(nameof(Index));
        }

        // Hủy công đoạn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            var congDoan = await _context.CongDoanXuLys.FindAsync(id);

            if (congDoan == null)
            {
                return NotFound();
            }

            if (!_congDoanService.Huy(congDoan, out string thongBao))
            {
                TempData["Loi"] = thongBao;
                return RedirectToAction(nameof(Index));
            }

            await _context.SaveChangesAsync();

            TempData["ThongBao"] = thongBao;
            return RedirectToAction(nameof(Index));
        }
    }
}
