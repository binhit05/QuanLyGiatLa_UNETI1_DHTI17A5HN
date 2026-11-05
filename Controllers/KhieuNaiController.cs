// Họ và tên: Lê Duy Khánh
// Mã sinh viên: 23103100216
// Nội dung thực hiện: Tiếp nhận và xử lý khiếu nại của khách hàng.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Filters;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers;

[PhanQuyen(VaiTroConst.Admin, VaiTroConst.NhanVienTiepNhan)]
public class KhieuNaiController : BaseController
{
    public KhieuNaiController(AppDbContext db) : base(db) { }

    public IActionResult Index(string? q, string? trangThai, string? sort, int page = 1)
    {
        var query = _db.KhieuNais.Include(k => k.DonGiat).ThenInclude(d => d!.KhachHang).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim(); var id = UiHelper.ThamSoMa(s, "KN-"); var idDon = UiHelper.ThamSoMa(s, "DG-");
            query = query.Where(k => k.NoiDung.Contains(s) || k.DonGiat!.KhachHang!.HoTen.Contains(s)
                                     || (id != null && k.MaKhieuNai == id) || (idDon != null && k.MaDonGiat == idDon));
        }
        if (trangThai == "chua") query = query.Where(k => k.TrangThai != "Đã giải quyết");
        else if (!string.IsNullOrEmpty(trangThai)) query = query.Where(k => k.TrangThai == trangThai);
        query = sort == "cu" ? query.OrderBy(k => k.NgayTao) : sort == "muc" ? query.OrderBy(k => k.MucDo == "Cao" ? 0 : k.MucDo == "Trung bình" ? 1 : 2).ThenByDescending(k => k.NgayTao)
              : query.OrderByDescending(k => k.NgayTao);

        ViewBag.Filter = new FilterBarVM
        {
            Placeholder = "Tìm kiếm theo mã, tên, nội dung...", Q = q, Sort = sort ?? "moi",
            Selects = new List<SelectBox> { new SelectBox { Name = "trangThai", AllText = "Tất cả trạng thái", Selected = trangThai,
                Opts = new List<Opt> { new Opt("chua", "Chưa giải quyết"), new Opt("Mới tiếp nhận", "Mới tiếp nhận"), new Opt("Đang xử lý", "Đang xử lý"), new Opt("Đã giải quyết", "Đã giải quyết") } } },
            Sorts = new List<Opt> { new Opt("moi", "Mới nhất"), new Opt("cu", "Cũ nhất"), new Opt("muc", "Mức độ cao trước") }
        };
        return View(PagedList<KhieuNai>.Tao(query, page, CoTrang));
    }

    private void NapForm()
    {
        var dons = _db.DonGiats.Include(d => d.KhachHang).Where(d => d.TrangThai != TrangThaiDon.DaHuy && d.TrangThai != TrangThaiDon.MoiTiepNhan)
                      .OrderByDescending(d => d.MaDonGiat).Take(150).ToList();
        ViewBag.Dons = dons.Select(d => new Opt(d.MaDonGiat.ToString(), UiHelper.MaDon(d) + " — " + d.KhachHang!.HoTen)).ToList();
        ViewBag.NguoiPhuTrachs = _db.TaiKhoans.Where(t => t.TrangThai && t.VaiTro != VaiTroConst.NhanVienXuLy).OrderBy(t => t.HoTen).ToList();
    }

    [HttpGet]
    public IActionResult Create(int? donId)
    {
        NapForm();
        return View("Form", new KhieuNai { MaDonGiat = donId ?? 0, NguoiPhuTrach = TenNguoiDung });
    }

    [HttpPost]
    public IActionResult Create(KhieuNai model)
    {
        var don = _db.DonGiats.Find(model.MaDonGiat);
        if (model.MaDonGiat > 0 && (don == null || don.TrangThai == TrangThaiDon.DaHuy)) ModelState.AddModelError("MaDonGiat", "Đơn giặt không hợp lệ hoặc đã hủy");
        if (!ModelState.IsValid) { NapForm(); return View("Form", model); }
        model.MaKhieuNai = 0; model.NgayTao = DateTime.Now; model.TrangThai = "Mới tiếp nhận"; model.DonGiat = null;
        _db.KhieuNais.Add(model);
        GhiLichSu(model.MaDonGiat, "Tiếp nhận khiếu nại: " + model.NoiDung);
        _db.SaveChanges();
        ThongBao("Đã tiếp nhận khiếu nại");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var x = _db.KhieuNais.Find(id);
        if (x == null) return NotFound();
        NapForm();
        return View("Form", x);
    }

    [HttpPost]
    public IActionResult Edit(KhieuNai model)
    {
        var x = _db.KhieuNais.Find(model.MaKhieuNai);
        if (x == null) return NotFound();
        if (model.TrangThai == "Đã giải quyết" && string.IsNullOrWhiteSpace(model.HuongXuLy))
            ModelState.AddModelError("HuongXuLy", "Nhập hướng xử lý trước khi đánh dấu đã giải quyết");
        if (!ModelState.IsValid) { NapForm(); model.MaDonGiat = x.MaDonGiat; return View("Form", model); }
        x.NoiDung = model.NoiDung; x.MucDo = model.MucDo; x.NguoiPhuTrach = model.NguoiPhuTrach; x.HuongXuLy = model.HuongXuLy;
        if (x.TrangThai != model.TrangThai)
        {
            GhiLichSu(x.MaDonGiat, "Khiếu nại " + UiHelper.MaKN(x.MaKhieuNai) + ": " + x.TrangThai + " → " + model.TrangThai);
            x.TrangThai = model.TrangThai;
            x.NgayHoanTat = model.TrangThai == "Đã giải quyết" ? DateTime.Now : (DateTime?)null;
        }
        _db.SaveChanges();
        ThongBao("Cập nhật khiếu nại thành công");
        return RedirectToAction(nameof(Index));
    }

    [PhanQuyen(VaiTroConst.Admin)]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var x = _db.KhieuNais.Find(id);
        if (x == null) return NotFound();
        _db.KhieuNais.Remove(x);
        _db.SaveChanges();
        ThongBao("Đã xóa khiếu nại");
        return RedirectToAction(nameof(Index));
    }
}
