// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: Đăng nhập, đăng xuất, phân quyền và quản lý tài khoản.
using Microsoft.AspNetCore.Mvc;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Filters;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers;

public class TaiKhoanController : BaseController
{
    public TaiKhoanController(AppDbContext db) : base(db) { }

    // ================= Đăng nhập / đăng xuất =================
    [HttpGet]
    public IActionResult DangNhap()
    {
        if (HttpContext.Session.LayMaTaiKhoan() != null) return RedirectToAction("Index", "ThongKe");
        return View();
    }

    [HttpPost]
    public IActionResult DangNhap(DangNhapViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var tk = _db.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == vm.TenDangNhap && t.MatKhau == vm.MatKhau);
        if (tk == null) { ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu"); return View(vm); }
        if (!tk.TrangThai) { ModelState.AddModelError("", "Tài khoản đã bị khóa, vui lòng liên hệ quản trị viên"); return View(vm); }

        tk.LanDangNhapCuoi = DateTime.Now;
        _db.SaveChanges();

        HttpContext.Session.SetInt32(SessionHelper.KeyMa, tk.MaTaiKhoan);
        HttpContext.Session.SetString(SessionHelper.KeyTen, tk.HoTen);
        HttpContext.Session.SetString(SessionHelper.KeyVaiTro, tk.VaiTro);
        return RedirectToAction("Index", "ThongKe");
    }

    public IActionResult DangXuat()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("DangNhap");
    }

    public IActionResult TuChoiTruyCap() => View();

    // ================= Quản lý tài khoản (Admin) =================
    [PhanQuyen(VaiTroConst.Admin)]
    public IActionResult Index(string? q, string? trangThai, string? sort, int page = 1)
    {
        var query = _db.TaiKhoans.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim(); var id = UiHelper.ThamSoMa(s, "TK-");
            query = query.Where(t => t.HoTen.Contains(s) || t.TenDangNhap.Contains(s)
                                     || (t.Email != null && t.Email.Contains(s)) || (id != null && t.MaTaiKhoan == id));
        }
        if (trangThai == "1") query = query.Where(t => t.TrangThai);
        else if (trangThai == "0") query = query.Where(t => !t.TrangThai);

        query = sort switch
        {
            "cu" => query.OrderBy(t => t.MaTaiKhoan),
            "ten" => query.OrderBy(t => t.HoTen),
            _ => query.OrderByDescending(t => t.MaTaiKhoan)
        };

        ViewBag.Filter = new FilterBarVM
        {
            Placeholder = "Tìm kiếm theo mã, tên, tên đăng nhập...",
            Q = q, Sort = sort ?? "moi",
            Selects = new List<SelectBox> { new SelectBox { Name = "trangThai", AllText = "Tất cả trạng thái", Selected = trangThai,
                Opts = new List<Opt> { new Opt("1", "Hoạt động"), new Opt("0", "Đã khóa") } } }
        };
        return View(PagedList<TaiKhoan>.Tao(query, page, CoTrang));
    }

    [PhanQuyen(VaiTroConst.Admin)]
    [HttpGet]
    public IActionResult Create() => View("Form", new TaiKhoanFormVM { VaiTro = VaiTroConst.NhanVienTiepNhan });

    [PhanQuyen(VaiTroConst.Admin)]
    [HttpPost]
    public IActionResult Create(TaiKhoanFormVM vm)
    {
        if (string.IsNullOrWhiteSpace(vm.MatKhau)) ModelState.AddModelError("MatKhau", "Nhập mật khẩu (tối thiểu 6 ký tự)");
        if (_db.TaiKhoans.Any(t => t.TenDangNhap == vm.TenDangNhap)) ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại");
        if (!ModelState.IsValid) return View("Form", vm);

        _db.TaiKhoans.Add(new TaiKhoan
        {
            TenDangNhap = vm.TenDangNhap.Trim(), MatKhau = vm.MatKhau!, HoTen = vm.HoTen.Trim(),
            Email = vm.Email, VaiTro = vm.VaiTro, TrangThai = vm.TrangThai
        });
        _db.SaveChanges();
        ThongBao("Tạo tài khoản thành công");
        return RedirectToAction(nameof(Index));
    }

    [PhanQuyen(VaiTroConst.Admin)]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var t = _db.TaiKhoans.Find(id);
        if (t == null) return NotFound();
        return View("Form", new TaiKhoanFormVM { MaTaiKhoan = t.MaTaiKhoan, TenDangNhap = t.TenDangNhap, HoTen = t.HoTen, Email = t.Email, VaiTro = t.VaiTro, TrangThai = t.TrangThai });
    }

    [PhanQuyen(VaiTroConst.Admin)]
    [HttpPost]
    public IActionResult Edit(TaiKhoanFormVM vm)
    {
        var t = _db.TaiKhoans.Find(vm.MaTaiKhoan);
        if (t == null) return NotFound();
        if (_db.TaiKhoans.Any(x => x.TenDangNhap == vm.TenDangNhap && x.MaTaiKhoan != vm.MaTaiKhoan))
            ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại");

        bool laChinhMinh = t.MaTaiKhoan == MaNguoiDung;
        if (laChinhMinh && (!vm.TrangThai || vm.VaiTro != VaiTroConst.Admin))
            ModelState.AddModelError("", "Bạn không thể tự khóa hoặc tự hạ quyền tài khoản của chính mình");
        // phải còn ít nhất một Admin đang hoạt động
        if (t.VaiTro == VaiTroConst.Admin && (vm.VaiTro != VaiTroConst.Admin || !vm.TrangThai)
            && !_db.TaiKhoans.Any(x => x.VaiTro == VaiTroConst.Admin && x.TrangThai && x.MaTaiKhoan != t.MaTaiKhoan))
            ModelState.AddModelError("", "Hệ thống phải còn ít nhất một Admin đang hoạt động");
        if (!ModelState.IsValid) return View("Form", vm);

        t.TenDangNhap = vm.TenDangNhap.Trim(); t.HoTen = vm.HoTen.Trim(); t.Email = vm.Email;
        t.VaiTro = vm.VaiTro; t.TrangThai = vm.TrangThai;
        if (!string.IsNullOrWhiteSpace(vm.MatKhau)) t.MatKhau = vm.MatKhau;
        _db.SaveChanges();
        ThongBao("Cập nhật tài khoản thành công");
        return RedirectToAction(nameof(Index));
    }

    [PhanQuyen(VaiTroConst.Admin)]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var t = _db.TaiKhoans.Find(id);
        if (t == null) return NotFound();
        if (t.MaTaiKhoan == MaNguoiDung) { BaoLoi("Không thể xóa tài khoản đang đăng nhập"); return RedirectToAction(nameof(Index)); }
        if (_db.NhanViens.Any(n => n.MaTaiKhoan == id) || _db.DonGiats.Any(d => d.MaNguoiTiepNhan == id))
        {
            BaoLoi("Tài khoản đã gắn với nhân viên hoặc đơn giặt nên không thể xóa. Hãy chuyển sang trạng thái khóa.");
            return RedirectToAction(nameof(Index));
        }
        _db.TaiKhoans.Remove(t);
        _db.SaveChanges();
        ThongBao("Đã xóa tài khoản");
        return RedirectToAction(nameof(Index));
    }
}
