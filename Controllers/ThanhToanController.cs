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
    // Thứ tự xử lý: Truy vấn -> Tìm kiếm -> Lọc -> Sắp xếp -> Phân trang (Skip/Take trong PagedList) -> Hiển thị.
    // Toàn bộ chạy phía SQL qua IQueryable, KHÔNG gọi ToList() trước khi phân trang.
    public IActionResult Index(string? q, string? trangThai, string? sort, int page = 1)
    {
        var query = _db.DonGiats.Where(d => d.TrangThai != TrangThaiDon.DaHuy);

        // Tìm kiếm: mã đơn (DG-...), mã thanh toán (TT-...), tên hoặc số điện thoại khách hàng
        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim();
            int? idDon = UiHelper.ThamSoMa(s, "DG-");
            int? idTT = s.StartsWith("TT-", StringComparison.OrdinalIgnoreCase) && UiHelper.ThamSoMa(s, "TT-") is int so ? so - 25000 : (int?)null; // MaTT = 25000 + MaDonGiat
            query = query.Where(d => (idDon != null && d.MaDonGiat == idDon) || (idTT != null && d.MaDonGiat == idTT)
                                     || d.KhachHang!.HoTen.Contains(s) || d.KhachHang.SoDienThoai.Contains(s));
        }

        // Lọc theo trạng thái thanh toán (tính bằng SUM các khoản thanh toán của đơn, cùng quy tắc với DonGiat.TrangThaiThanhToan)
        switch (trangThai)
        {
            case "Chưa thanh toán":
                query = query.Where(d => !d.ThanhToans.Any()); break;
            case "Một phần":
                query = query.Where(d => d.ThanhToans.Any() && d.TongTienDuKien - d.ThanhToans.Sum(t => t.SoTien) > 0); break;
            case "Đã thanh toán":
                query = query.Where(d => d.ThanhToans.Any() && d.TongTienDuKien - d.ThanhToans.Sum(t => t.SoTien) <= 0); break;
        }

        // Sắp xếp (số còn lại = TongTienDuKien - tổng đã thanh toán, tính trong SQL)
        query = sort switch
        {
            "cu" => query.OrderBy(d => d.MaDonGiat),
            "no" => query.OrderByDescending(d => d.TongTienDuKien - d.ThanhToans.Sum(t => t.SoTien)).ThenByDescending(d => d.MaDonGiat),
            _ => query.OrderByDescending(d => d.MaDonGiat)
        };

        ViewBag.Filter = new FilterBarVM
        {
            Placeholder = "Tìm kiếm theo mã, tên, số điện thoại...",
            Q = q,
            Sort = sort ?? "moi",
            Selects = new List<SelectBox> { new SelectBox { Name = "trangThai", AllText = "Tất cả trạng thái", Selected = trangThai,
                Opts = new List<Opt> { new Opt("Đã thanh toán", "Đã thanh toán"), new Opt("Một phần", "Một phần"), new Opt("Chưa thanh toán", "Chưa thanh toán") } } },
            Sorts = new List<Opt> { new Opt("moi", "Mới nhất"), new Opt("cu", "Cũ nhất"), new Opt("no", "Còn nợ nhiều nhất") }
        };
        // Include sau cùng: chỉ nạp khách hàng + thanh toán của các đơn nằm trong trang hiện tại
        return View(PagedList<DonGiat>.Tao(query.Include(d => d.KhachHang).Include(d => d.ThanhToans), page, CoTrang));
    }

    // Nạp danh sách đơn còn công nợ cho form: lọc ngay trong SQL (còn lại > 0), không tải toàn bộ đơn rồi lọc trong bộ nhớ
    private void NapForm()
    {
        var dons = _db.DonGiats.Include(d => d.KhachHang).Include(d => d.ThanhToans)
            .Where(d => d.TrangThai != TrangThaiDon.DaHuy && d.TongTienDuKien - d.ThanhToans.Sum(t => t.SoTien) > 0)
            .OrderByDescending(d => d.MaDonGiat).ToList();
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
