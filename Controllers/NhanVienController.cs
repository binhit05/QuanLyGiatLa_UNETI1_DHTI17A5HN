// Họ và tên: Nghiêm Xuân Bằng
// Mã sinh viên: .....................
// Nội dung thực hiện: Quản lý hồ sơ nhân viên.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Filters;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers;

[PhanQuyen(VaiTroConst.Admin)]
public class NhanVienController : BaseController
{
    public NhanVienController(AppDbContext db) : base(db) { }

    public IActionResult Index(string? q, string? trangThai, string? sort, int page = 1)
    {
        var query = _db.NhanViens.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim(); var id = UiHelper.ThamSoMa(s, "NV-");
            query = query.Where(x => x.HoTen.Contains(s) || (x.SoDienThoai != null && x.SoDienThoai.Contains(s))
                                     || (x.Email != null && x.Email.Contains(s)) || (id != null && x.MaNhanVien == id));
        }
        if (trangThai == "1") query = query.Where(x => x.TrangThai); else if (trangThai == "0") query = query.Where(x => !x.TrangThai);
        query = sort == "ten" ? query.OrderBy(x => x.HoTen) : sort == "cu" ? query.OrderBy(x => x.MaNhanVien) : query.OrderBy(x => x.MaNhanVien);

        var ds = PagedList<NhanVien>.Tao(query, page, CoTrang);
        var ids = ds.Items.Select(x => x.MaNhanVien).ToList();
        ViewBag.DangPhuTrach = _db.PhanCongs
            .Where(p => ids.Contains(p.MaNhanVien) && (p.DonGiat!.TrangThai == TrangThaiDon.DangXuLy || p.DonGiat!.TrangThai == TrangThaiDon.DaXacNhan))
            .GroupBy(p => p.MaNhanVien).Select(g => new { g.Key, Dem = g.Count() }).ToDictionary(x => x.Key, x => x.Dem);

        ViewBag.Filter = new FilterBarVM
        {
            Placeholder = "Tìm kiếm theo mã, tên, số điện thoại...", Q = q, Sort = sort ?? "moi",
            Selects = new List<SelectBox> { new SelectBox { Name = "trangThai", AllText = "Tất cả trạng thái", Selected = trangThai,
                Opts = new List<Opt> { new Opt("1", "Hoạt động"), new Opt("0", "Tạm nghỉ") } } },
            Sorts = new List<Opt> { new Opt("moi", "Theo mã"), new Opt("ten", "Tên A → Z") }
        };
        return View(ds);
    }

    private void NapForm(int? nvId)
    {
        // tài khoản chưa gắn với nhân viên nào (hoặc đang gắn với chính nhân viên này)
        var daGan = _db.NhanViens.Where(n => n.MaTaiKhoan != null && n.MaNhanVien != nvId).Select(n => n.MaTaiKhoan!.Value).ToList();
        ViewBag.TaiKhoans = _db.TaiKhoans.Where(t => !daGan.Contains(t.MaTaiKhoan)).OrderBy(t => t.HoTen).ToList();
    }

    [HttpGet]
    public IActionResult Create() { NapForm(null); return View("Form", new NhanVien { ChucVu = "Nhân viên xử lý" }); }

    [HttpPost]
    public IActionResult Create(NhanVien model)
    {
        if (!ModelState.IsValid) { NapForm(null); return View("Form", model); }
        model.MaNhanVien = 0;
        _db.NhanViens.Add(model);
        _db.SaveChanges();
        ThongBao("Thêm nhân viên thành công");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var x = _db.NhanViens.Find(id);
        if (x == null) return NotFound();
        NapForm(id);
        return View("Form", x);
    }

    [HttpPost]
    public IActionResult Edit(NhanVien model)
    {
        var x = _db.NhanViens.Find(model.MaNhanVien);
        if (x == null) return NotFound();
        if (!ModelState.IsValid) { NapForm(model.MaNhanVien); return View("Form", model); }
        x.HoTen = model.HoTen.Trim(); x.SoDienThoai = model.SoDienThoai; x.Email = model.Email;
        x.ChucVu = model.ChucVu; x.MaTaiKhoan = model.MaTaiKhoan; x.TrangThai = model.TrangThai;
        _db.SaveChanges();
        ThongBao("Cập nhật nhân viên thành công");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var x = _db.NhanViens.Find(id);
        if (x == null) return NotFound();
        if (_db.PhanCongs.Any(p => p.MaNhanVien == id) || _db.CongDoans.Any(c => c.MaNhanVien == id))
        {
            BaoLoi("Nhân viên đã có phân công xử lý nên không thể xóa. Hãy chuyển sang trạng thái tạm nghỉ.");
            return RedirectToAction(nameof(Index));
        }
        _db.NhanViens.Remove(x);
        _db.SaveChanges();
        ThongBao("Đã xóa nhân viên");
        return RedirectToAction(nameof(Index));
    }
}
