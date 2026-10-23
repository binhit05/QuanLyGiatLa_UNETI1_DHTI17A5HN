// Họ và tên: Lê Duy Khánh
// Mã sinh viên: 23103100216
// Nội dung thực hiện: Ghi nhận thanh toán và đối soát công nợ.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Filters;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers;

[PhanQuyen(VaiTroConst.Admin, VaiTroConst.NhanVienTiepNhan)]
public class ThanhToanController : BaseController
{
    public ThanhToanController(AppDbContext db) : base(db) { }

    public static readonly string[] PhuongThucs = { "Tiền mặt", "Chuyển khoản", "Quét QR" };

    // Danh sách theo từng đơn giặt (tổng hợp công nợ)
    public IActionResult Index(string? q, string? trangThai, string? sort, int page = 1)
    {
        var ds = _db.DonGiats.Include(d => d.KhachHang).Include(d => d.ThanhToans)
                   .Where(d => d.TrangThai != TrangThaiDon.DaHuy).ToList();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim().ToLower();
            ds = ds.Where(d => UiHelper.MaDon(d).ToLower().Contains(s) || UiHelper.MaTT(d.MaDonGiat).ToLower().Contains(s)
                               || d.KhachHang!.HoTen.ToLower().Contains(s) || d.KhachHang.SoDienThoai.Contains(s)).ToList();
        }
        if (!string.IsNullOrEmpty(trangThai)) ds = ds.Where(d => d.TrangThaiThanhToan == trangThai).ToList();
        ds = sort == "cu" ? ds.OrderBy(d => d.MaDonGiat).ToList()
           : sort == "no" ? ds.OrderByDescending(d => d.ConLai).ToList()
           : ds.OrderByDescending(d => d.MaDonGiat).ToList();

        ViewBag.Filter = new FilterBarVM
        {
            Placeholder = "Tìm kiếm theo mã, tên, số điện thoại...", Q = q, Sort = sort ?? "moi",
            Selects = new List<SelectBox> { new SelectBox { Name = "trangThai", AllText = "Tất cả trạng thái", Selected = trangThai,
                Opts = new List<Opt> { new Opt("Đã thanh toán", "Đã thanh toán"), new Opt("Một phần", "Một phần"), new Opt("Chưa thanh toán", "Chưa thanh toán") } } },
            Sorts = new List<Opt> { new Opt("moi", "Mới nhất"), new Opt("cu", "Cũ nhất"), new Opt("no", "Còn nợ nhiều nhất") }
        };
        return View(PagedList<DonGiat>.Tao(ds.AsQueryable(), page, CoTrang));
    }

    private void NapForm()
    {
        var dons = _db.DonGiats.Include(d => d.KhachHang).Include(d => d.ThanhToans)
            .Where(d => d.TrangThai != TrangThaiDon.DaHuy).OrderByDescending(d => d.MaDonGiat).ToList()
            .Where(d => d.ConLai > 0).ToList();
        ViewBag.Dons = dons.Select(d => new Opt(d.MaDonGiat.ToString(), UiHelper.MaDon(d) + " — " + d.KhachHang!.HoTen + " (còn " + UiHelper.Tien(d.ConLai) + ")")).ToList();
        ViewBag.ConLaiJson = System.Text.Json.JsonSerializer.Serialize(dons.Select(d => new { id = d.MaDonGiat, conLai = d.ConLai }));
    }

    [HttpGet]
    public IActionResult Create(int? donId)
    {
        NapForm();
        var vm = new ThanhToanVM { MaDonGiat = donId ?? 0 };
        if (donId != null)
        {
            var d = _db.DonGiats.Include(x => x.ThanhToans).FirstOrDefault(x => x.MaDonGiat == donId);
            if (d != null) vm.SoTien = Math.Max(0, d.ConLai);
        }
        return View(vm);
    }

    [HttpPost]
    public IActionResult Create(ThanhToanVM vm)
    {
        var d = _db.DonGiats.Include(x => x.ThanhToans).FirstOrDefault(x => x.MaDonGiat == vm.MaDonGiat);
        if (ModelState.IsValid)
        {
            if (d == null) ModelState.AddModelError("MaDonGiat", "Đơn giặt không tồn tại");
            else if (d.TrangThai == TrangThaiDon.DaHuy) ModelState.AddModelError("MaDonGiat", "Đơn đã hủy, không thể thanh toán");
            else if (vm.SoTien > d.ConLai) ModelState.AddModelError("SoTien", "Số tiền vượt quá số còn phải thu (" + UiHelper.Tien(d.ConLai) + ")");
            if (!PhuongThucs.Contains(vm.PhuongThuc)) ModelState.AddModelError("PhuongThuc", "Phương thức không hợp lệ");
        }
        if (!ModelState.IsValid) { NapForm(); return View(vm); }

        _db.ThanhToans.Add(new ThanhToan { MaDonGiat = d!.MaDonGiat, SoTien = vm.SoTien, PhuongThuc = vm.PhuongThuc, GhiChu = vm.GhiChu, NguoiThucHien = TenNguoiDung, NgayThanhToan = DateTime.Now });
        GhiLichSu(d.MaDonGiat, "Ghi nhận thanh toán " + UiHelper.Tien(vm.SoTien) + " (" + vm.PhuongThuc + ")");
        _db.SaveChanges();
        ThongBao("Ghi nhận thanh toán thành công");
        return RedirectToAction(nameof(ChiTiet), new { id = d.MaDonGiat });
    }

    // Các khoản thanh toán của một đơn
    public IActionResult ChiTiet(int id)
    {
        var d = _db.DonGiats.Include(x => x.KhachHang).Include(x => x.ThanhToans).FirstOrDefault(x => x.MaDonGiat == id);
        return d == null ? NotFound() : View(d);
    }

    [PhanQuyen(VaiTroConst.Admin)]
    [HttpPost]
    public IActionResult XoaKhoan(int id)
    {
        var t = _db.ThanhToans.Find(id);
        if (t == null) return NotFound();
        int don = t.MaDonGiat;
        var d = _db.DonGiats.Find(don);
        if (d != null && d.TrangThai == TrangThaiDon.HoanThanh) { BaoLoi("Đơn đã hoàn thành (đã giao trả) nên không thể xóa khoản thanh toán."); return RedirectToAction(nameof(ChiTiet), new { id = don }); }
        GhiLichSu(don, "Xóa khoản thanh toán " + UiHelper.Tien(t.SoTien));
        _db.ThanhToans.Remove(t);
        _db.SaveChanges();
        ThongBao("Đã xóa khoản thanh toán");
        return RedirectToAction(nameof(ChiTiet), new { id = don });
    }

    [PhanQuyen(VaiTroConst.Admin)]
    [HttpPost]
    public IActionResult XoaTatCa(int id)
    {
        var d = _db.DonGiats.Include(x => x.ThanhToans).FirstOrDefault(x => x.MaDonGiat == id);
        if (d == null) return NotFound();
        if (d.TrangThai == TrangThaiDon.HoanThanh) { BaoLoi("Đơn đã hoàn thành (đã giao trả) nên không thể xóa thanh toán."); return RedirectToAction(nameof(Index)); }
        if (!d.ThanhToans.Any()) { BaoLoi("Đơn chưa có khoản thanh toán nào."); return RedirectToAction(nameof(Index)); }
        _db.ThanhToans.RemoveRange(d.ThanhToans);
        GhiLichSu(id, "Xóa toàn bộ khoản thanh toán của đơn");
        _db.SaveChanges();
        ThongBao("Đã xóa các khoản thanh toán của đơn");
        return RedirectToAction(nameof(Index));
    }
}
