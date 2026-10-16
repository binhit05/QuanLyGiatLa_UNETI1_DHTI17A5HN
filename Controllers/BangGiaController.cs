using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers
{
    public class BangGiaController : Controller
    {
        private readonly AppDbContext _context;

        public BangGiaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: BangGia
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            ViewData["CurrentFilter"] = searchString;

            var query = _context.BangGiaDichVu
                .Include(b => b.DichVu)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(b => b.DichVu != null && b.DichVu.TenDichVu.Contains(searchString));
            }

            query = query.OrderByDescending(b => b.MaBangGia);

            int pageSize = 10;
            var paginatedList = await PaginatedList<BangGiaDichVu>.CreateAsync(query, pageNumber ?? 1, pageSize);

            return View(paginatedList);
        }

        // GET: BangGia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bangGia = await _context.BangGiaDichVu
                .Include(b => b.DichVu)
                .FirstOrDefaultAsync(m => m.MaBangGia == id);

            if (bangGia == null)
            {
                return NotFound();
            }

            return View(bangGia);
        }

        // GET: BangGia/Create
        public IActionResult Create()
        {
            ViewData["MaDichVu"] = new SelectList(_context.DichVu, "MaDichVu", "TenDichVu");
            return View();
        }

        // POST: BangGia/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaBangGia,MaDichVu,DonGia,NgayApDung,NgayKetThuc,TrangThai")] BangGiaDichVu bangGia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bangGia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDichVu"] = new SelectList(_context.DichVu, "MaDichVu", "TenDichVu", bangGia.MaDichVu);
            return View(bangGia);
        }

        // GET: BangGia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bangGia = await _context.BangGiaDichVu.FindAsync(id);
            if (bangGia == null)
            {
                return NotFound();
            }
            ViewData["MaDichVu"] = new SelectList(_context.DichVu, "MaDichVu", "TenDichVu", bangGia.MaDichVu);
            return View(bangGia);
        }

        // POST: BangGia/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaBangGia,MaDichVu,DonGia,NgayApDung,NgayKetThuc,TrangThai")] BangGiaDichVu bangGia)
        {
            if (id != bangGia.MaBangGia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bangGia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BangGiaExists(bangGia.MaBangGia))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDichVu"] = new SelectList(_context.DichVu, "MaDichVu", "TenDichVu", bangGia.MaDichVu);
            return View(bangGia);
        }

        // GET: BangGia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bangGia = await _context.BangGiaDichVu
                .Include(b => b.DichVu)
                .FirstOrDefaultAsync(m => m.MaBangGia == id);

            if (bangGia == null)
            {
                return NotFound();
            }

            return View(bangGia);
        }

        // POST: BangGia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bangGia = await _context.BangGiaDichVu.FindAsync(id);
            if (bangGia != null)
            {
                _context.BangGiaDichVu.Remove(bangGia);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BangGiaExists(int id)
        {
            return _context.BangGiaDichVu.Any(e => e.MaBangGia == id);
        }
    }
}