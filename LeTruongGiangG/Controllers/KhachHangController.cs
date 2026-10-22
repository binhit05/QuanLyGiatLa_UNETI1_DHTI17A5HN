using Microsoft.AspNetCore.Mvc;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Filters;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers;

[PhanQuyen(VaiTroConst.Admin, VaiTroConst.NhanVienTiepNhan)]
public class KhachHangController : BaseController
{
    public KhachHangController(AppDbContext db) : base(db) { }

    public IActionResult Index(string? q, string? trangThai, string? sort, int page = 1)
    {
        var query = _db.KhachHangs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim(); var id = UiHelper.ThamSoMa(s, "KH-");
            query = query.Where(x => x.HoTen.Contains(s) || x.SoDienThoai.Contains(s)
                                     || (x.Email != null && x.Email.Contains(s)) || (id != null && x.MaKhachHang == id));
        }
        if (trangThai == "1") query = query.Where(x => x.TrangThai); else if (trangThai == "0") query = query.Where(x => !x.TrangThai);
        query = sort == "ten" ? query.OrderBy(x => x.HoTen) : sort == "cu" ? query.OrderBy(x => x.MaKhachHang) : query.OrderByDescending(x => x.MaKhachHang);

        var ds = PagedList<KhachHang>.Tao(query, page, CoTrang);
        var ids = ds.Items.Select(x => x.MaKhachHang).ToList();
        var thongKe = _db.DonGiats.Where(d => ids.Contains(d.MaKhachHang) && d.TrangThai != TrangThaiDon.DaHuy)
                         .GroupBy(d => d.MaKhachHang)
                         .Select(g => new { g.Key, SoDon = g.Count(), Tong = g.Sum(d => d.TongTienDuKien) }).ToList();
        ViewBag.SoDon = thongKe.ToDictionary(x => x.Key, x => x.SoDon);
        ViewBag.TongChiTieu = thongKe.ToDictionary(x => x.Key, x => x.Tong);

        ViewBag.Filter = new FilterBarVM
        {
            Placeholder = "Tìm kiếm theo mã, tên, số điện thoại...",
            Q = q,
            Sort = sort ?? "moi",
            Selects = new List<SelectBox> { new SelectBox { Name = "trangThai", AllText = "Tất cả trạng thái", Selected = trangThai,
                Opts = new List<Opt> { new Opt("1", "Hoạt động"), new Opt("0", "Ngừng hoạt động") } } }
        };
        return View(ds);
    }

    // Thêm nhanh từ cửa sổ (modal) trên trang danh sách
    [HttpPost]
    public IActionResult Create(KhachHang model)
    {
        if (_db.KhachHangs.Any(x => x.SoDienThoai == model.SoDienThoai)) ModelState.AddModelError("SoDienThoai", "Số điện thoại đã tồn tại");
        if (!ModelState.IsValid)
        {
            var loi = string.Join(" · ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            BaoLoi("Không thêm được khách hàng: " + loi);
            return RedirectToAction(nameof(Index), new { them = 1 });
        }
        model.MaKhachHang = 0;
        _db.KhachHangs.Add(model);
        _db.SaveChanges();
        ThongBao("Thêm khách hàng thành công");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var x = _db.KhachHangs.Find(id);
        return x == null ? NotFound() : View(x);
    }

    [HttpPost]
    public IActionResult Edit(KhachHang model)
    {
        var x = _db.KhachHangs.Find(model.MaKhachHang);
        if (x == null) return NotFound();
        if (_db.KhachHangs.Any(k => k.SoDienThoai == model.SoDienThoai && k.MaKhachHang != model.MaKhachHang))
            ModelState.AddModelError("SoDienThoai", "Số điện thoại đã tồn tại");
        if (!ModelState.IsValid) return View(model);
        x.HoTen = model.HoTen.Trim(); x.SoDienThoai = model.SoDienThoai; x.Email = model.Email;
        x.DiaChi = model.DiaChi; x.GhiChu = model.GhiChu; x.TrangThai = model.TrangThai;
        _db.SaveChanges();
        ThongBao("Cập nhật khách hàng thành công");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var x = _db.KhachHangs.Find(id);
        if (x == null) return NotFound();
        if (_db.DonGiats.Any(d => d.MaKhachHang == id))
        {
            BaoLoi("Khách hàng đã có đơn giặt nên không thể xóa. Hãy chuyển sang ngừng hoạt động.");
            return RedirectToAction(nameof(Index));
        }
        _db.KhachHangs.Remove(x);
        _db.SaveChanges();
        ThongBao("Đã xóa khách hàng");
        return RedirectToAction(nameof(Index));
    }
}
