

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers
{
    public class DichVuController : Controller
    {
        private readonly AppDbContext _context;

        public DichVuController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DichVu
        public async Task<IActionResult> Index(
            string searchString,
            bool? statusFilter,
            string sortOrder,
            int? pageNumber)
        {
            // Lưu lại các tham số sắp xếp & lọc ra ViewData để dùng trên View
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CurrentFilter"] = searchString;
            ViewData["StatusFilter"] = statusFilter;

            var dichVus = _context.DichVu.AsQueryable();

            // 1. Tìm kiếm (Search) theo Tên dịch vụ
            if (!string.IsNullOrEmpty(searchString))
            {
                dichVus = dichVus.Where(d => d.TenDichVu.Contains(searchString));
            }

            // 2. Lọc (Filter) theo Trạng thái
            if (statusFilter.HasValue)
            {
                dichVus = dichVus.Where(d => d.TrangThai == statusFilter.Value);
            }

            // 3. Sắp xếp (Sort) theo tên (A-Z hoặc Z-A)
            dichVus = sortOrder switch
            {
                "name_desc" => dichVus.OrderByDescending(d => d.TenDichVu),
                _ => dichVus.OrderBy(d => d.TenDichVu),
            };

            // 4. Phân trang (Pagination) - 5 dòng / trang
            int pageSize = 5;
            return View(await PaginatedList<DichVu>.CreateAsync(dichVus.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: DichVu/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DichVu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DichVu dichVu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dichVu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dichVu);
        }

        // GET: DichVu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var dichVu = await _context.DichVu.FindAsync(id);
            if (dichVu == null) return NotFound();

            return View(dichVu);
        }

        // POST: DichVu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DichVu dichVu)
        {
            if (id != dichVu.MaDichVu) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dichVu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DichVu.Any(e => e.MaDichVu == dichVu.MaDichVu))
                        return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dichVu);
        }
    }
}