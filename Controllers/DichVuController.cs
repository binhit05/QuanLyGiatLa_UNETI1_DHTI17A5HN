// Họ và tên: Nghiêm Xuân Bằng
// Mã sinh viên: .....................
// Nội dung thực hiện: Quản lý dịch vụ giặt là.
using Microsoft.AspNetCore.Mvc;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Filters;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers;

[PhanQuyen(VaiTroConst.Admin)]
public class DichVuController : BaseController
{
    public DichVuController(AppDbContext db) : base(db) { }

    public IActionResult Index(string? q, string? trangThai, string? sort, int page = 1)
    {
        var query = _db.DichVus.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim(); var id = UiHelper.ThamSoMa(s, "DV-");
            query = query.Where(x => x.TenDichVu.Contains(s) || (x.MoTa != null && x.MoTa.Contains(s)) || (id != null && x.MaDichVu == id));
        }
        if (trangThai == "1") query = query.Where(x => x.TrangThai); else if (trangThai == "0") query = query.Where(x => !x.TrangThai);
        query = sort == "ten" ? query.OrderBy(x => x.TenDichVu) : sort == "cu" ? query.OrderBy(x => x.MaDichVu) : query.OrderBy(x => x.MaDichVu);

        var ds = PagedList<DichVu>.Tao(query, page, CoTrang);
        var ids = ds.Items.Select(x => x.MaDichVu).ToList();
        var bg = _db.BangGias.Include(b => b.DonViTinh).Where(b => ids.Contains(b.MaDichVu) && b.TrangThai).ToList();
        ViewBag.Gia = bg.GroupBy(b => b.MaDichVu).ToDictionary(g => g.Key, g => g.OrderBy(b => b.DonGia).First());
        ViewBag.SoGia = bg.GroupBy(b => b.MaDichVu).ToDictionary(g => g.Key, g => g.Count());

        ViewBag.Filter = new FilterBarVM
        {
            Placeholder = "Tìm kiếm theo mã, tên dịch vụ...", Q = q, Sort = sort ?? "moi",
            Selects = new List<SelectBox> { new SelectBox { Name = "trangThai", AllText = "Tất cả trạng thái", Selected = trangThai,
                Opts = new List<Opt> { new Opt("1", "Hoạt động"), new Opt("0", "Ngừng hoạt động") } } },
            Sorts = new List<Opt> { new Opt("moi", "Theo mã"), new Opt("ten", "Tên A → Z") }
        };
        return View(ds);
    }

    [HttpGet] public IActionResult Create() => View("Form", new DichVu());

    [HttpPost]
    public IActionResult Create(DichVu model)
    {
        if (_db.DichVus.Any(x => x.TenDichVu == model.TenDichVu)) ModelState.AddModelError("TenDichVu", "Tên dịch vụ đã tồn tại");
        if (!ModelState.IsValid) return View("Form", model);
        model.MaDichVu = 0;
        _db.DichVus.Add(model);
        _db.SaveChanges();
        ThongBao("Thêm dịch vụ thành công. Hãy thêm bảng giá cho dịch vụ này.");
        return RedirectToAction("Index", "BangGia", new { dichVuId = model.MaDichVu });
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var x = _db.DichVus.Find(id);
        return x == null ? NotFound() : View("Form", x);
    }

    [HttpPost]
    public IActionResult Edit(DichVu model)
    {
        if (_db.DichVus.Any(x => x.TenDichVu == model.TenDichVu && x.MaDichVu != model.MaDichVu)) ModelState.AddModelError("TenDichVu", "Tên dịch vụ đã tồn tại");
        if (!ModelState.IsValid) return View("Form", model);
        var x = _db.DichVus.Find(model.MaDichVu);
        if (x == null) return NotFound();
        x.TenDichVu = model.TenDichVu.Trim(); x.MoTa = model.MoTa; x.ThoiGianXuLyDuKien = model.ThoiGianXuLyDuKien; x.TrangThai = model.TrangThai;
        _db.SaveChanges();
        ThongBao("Cập nhật dịch vụ thành công");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var x = _db.DichVus.Find(id);
        if (x == null) return NotFound();
        if (_db.ChiTietDonGiats.Any(c => c.MaDichVu == id))
        {
            BaoLoi("Dịch vụ đã được dùng trong đơn giặt nên không thể xóa. Hãy chuyển sang ngừng hoạt động.");
            return RedirectToAction(nameof(Index));
        }
        _db.BangGias.RemoveRange(_db.BangGias.Where(b => b.MaDichVu == id));
        _db.DichVus.Remove(x);
        _db.SaveChanges();
        ThongBao("Đã xóa dịch vụ cùng bảng giá của nó");
        return RedirectToAction(nameof(Index));
    }
}
