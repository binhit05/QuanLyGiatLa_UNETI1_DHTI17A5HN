// Họ và tên: Lê Hoàng Anh
// Mã sinh viên: 23103100214
// Nội dung thực hiện: Ghi nhận giao trả đồ cho khách hàng.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Filters;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers;

[PhanQuyen(VaiTroConst.Admin, VaiTroConst.NhanVienTiepNhan)]
public class GiaoTraController : BaseController
{
    public GiaoTraController(AppDbContext db) : base(db) { }

    public IActionResult Index(string? q, string? trangThai, string? sort, int page = 1)
    {
        var ds = _db.DonGiats.Include(d => d.KhachHang).Include(d => d.ThanhToans).Include(d => d.GiaoTra)
                   .Where(d => d.TrangThai == TrangThaiDon.ChoGiaoTra || d.TrangThai == TrangThaiDon.HoanThanh).ToList();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim().ToLower();
            ds = ds.Where(d => UiHelper.MaDon(d).ToLower().Contains(s) || d.KhachHang!.HoTen.ToLower().Contains(s) || d.KhachHang.SoDienThoai.Contains(s)).ToList();
        }
        if (!string.IsNullOrEmpty(trangThai)) ds = ds.Where(d => d.TrangThaiHienThi == trangThai).ToList();
        ds = sort == "cu" ? ds.OrderBy(d => d.NgayHenTra).ToList() : sort == "tien" ? ds.OrderByDescending(d => d.ConLai).ToList() : ds.OrderByDescending(d => d.NgayHenTra).ToList();

        ViewBag.Filter = new FilterBarVM
        {
            Placeholder = "Tìm kiếm theo mã đơn, tên, số điện thoại...", Q = q, Sort = sort ?? "moi",
            Selects = new List<SelectBox> { new SelectBox { Name = "trangThai", AllText = "Tất cả trạng thái", Selected = trangThai,
                Opts = new List<Opt> { new Opt("Chờ giao trả", "Chờ giao trả"), new Opt("Quá hạn", "Quá hạn"), new Opt("Hoàn thành", "Hoàn thành") } } },
            Sorts = new List<Opt> { new Opt("moi", "Hẹn trả mới nhất"), new Opt("cu", "Hẹn trả cũ nhất"), new Opt("tien", "Cần thu nhiều nhất") }
        };
        return View(PagedList<DonGiat>.Tao(ds.AsQueryable(), page, CoTrang));
    }

    private void NapDon(int? giuMa)
    {
        var dons = _db.DonGiats.Include(d => d.KhachHang).Include(d => d.ThanhToans)
            .Where(d => d.TrangThai == TrangThaiDon.ChoGiaoTra).OrderBy(d => d.NgayHenTra).ToList();
        ViewBag.Dons = dons.Select(d => new Opt(d.MaDonGiat.ToString(), UiHelper.MaDon(d) + " — " + d.KhachHang!.HoTen + " (cần thu " + UiHelper.Tien(d.ConLai) + ")")).ToList();
        ViewBag.KhachJson = System.Text.Json.JsonSerializer.Serialize(dons.Select(d => new { id = d.MaDonGiat, ten = d.KhachHang!.HoTen }));
    }

    [HttpGet]
    public IActionResult Create(int? donId)
    {
        NapDon(donId);
        var vm = new GiaoTraVM { MaDonGiat = donId ?? 0, NguoiGiao = TenNguoiDung };
        if (donId != null) { var d = _db.DonGiats.Include(x => x.KhachHang).FirstOrDefault(x => x.MaDonGiat == donId); if (d != null) vm.NguoiNhan = d.KhachHang!.HoTen; }
        return View(vm);
    }

    [HttpPost]
    public IActionResult Create(GiaoTraVM vm)
    {
        var d = _db.DonGiats.Include(x => x.ThanhToans).Include(x => x.GiaoTra).FirstOrDefault(x => x.MaDonGiat == vm.MaDonGiat);
        if (ModelState.IsValid)
        {
            if (d == null) ModelState.AddModelError("MaDonGiat", "Đơn giặt không tồn tại");
            else if (d.TrangThai != TrangThaiDon.ChoGiaoTra) ModelState.AddModelError("MaDonGiat", "Chỉ giao trả đơn ở trạng thái Chờ giao trả");
            else if (d.GiaoTra != null) ModelState.AddModelError("MaDonGiat", "Đơn này đã được giao trả");
            else if (d.ConLai > 0) ModelState.AddModelError("", "Đơn còn phải thu " + UiHelper.Tien(d.ConLai) + ". Hãy ghi nhận thanh toán đủ trước khi giao trả.");
        }
        if (!ModelState.IsValid) { NapDon(vm.MaDonGiat); return View(vm); }

        _db.GiaoTras.Add(new GiaoTra { MaDonGiat = d!.MaDonGiat, NgayGiaoTra = vm.NgayGiaoTra, NguoiGiao = vm.NguoiGiao, NguoiNhan = vm.NguoiNhan, HinhThucNhan = vm.HinhThucNhan, GhiChu = vm.GhiChu });
        d.TrangThai = TrangThaiDon.HoanThanh;
        GhiLichSu(d.MaDonGiat, "Đã giao trả đồ cho " + vm.NguoiNhan + " (" + vm.HinhThucNhan + ")");
        _db.SaveChanges();
        ThongBao("Ghi nhận giao trả thành công, đơn đã hoàn thành");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var g = _db.GiaoTras.Include(x => x.DonGiat).ThenInclude(d => d!.KhachHang).FirstOrDefault(x => x.MaDonGiat == id);
        if (g == null) return NotFound();
        return View(g);
    }

    [HttpPost]
    public IActionResult Edit(int maDonGiat, string nguoiNhan, string hinhThucNhan, string? ghiChu)
    {
        var g = _db.GiaoTras.FirstOrDefault(x => x.MaDonGiat == maDonGiat);
        if (g == null) return NotFound();
        if (string.IsNullOrWhiteSpace(nguoiNhan)) { BaoLoi("Nhập tên người nhận"); return RedirectToAction(nameof(Edit), new { id = maDonGiat }); }
        g.NguoiNhan = nguoiNhan.Trim(); g.HinhThucNhan = hinhThucNhan; g.GhiChu = ghiChu;
        GhiLichSu(maDonGiat, "Cập nhật thông tin giao trả");
        _db.SaveChanges();
        ThongBao("Cập nhật giao trả thành công");
        return RedirectToAction(nameof(Index));
    }

    // Hủy ghi nhận giao trả (chỉ Admin): đơn quay lại Chờ giao trả
    [PhanQuyen(VaiTroConst.Admin)]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var g = _db.GiaoTras.Include(x => x.DonGiat).FirstOrDefault(x => x.MaDonGiat == id);
        if (g == null) return NotFound();
        g.DonGiat!.TrangThai = TrangThaiDon.ChoGiaoTra;
        GhiLichSu(id, "Hủy ghi nhận giao trả, đơn quay lại Chờ giao trả");
        _db.GiaoTras.Remove(g);
        _db.SaveChanges();
        ThongBao("Đã hủy ghi nhận giao trả");
        return RedirectToAction(nameof(Index));
    }
}
