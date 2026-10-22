// Họ và tên: Nghiêm Xuân Bằng
// Mã sinh viên: .....................
// Nội dung thực hiện: Quản lý bảng giá dịch vụ, kiểm tra không chồng lấn thời gian áp dụng.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Filters;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers;

// Bảng giá của từng dịch vụ (kiểm tra không chồng lấn thời gian áp dụng)
[PhanQuyen(VaiTroConst.Admin)]
public class BangGiaController : BaseController
{
    public BangGiaController(AppDbContext db) : base(db) { }

    public IActionResult Index(int dichVuId)
    {
        var dv = _db.DichVus.Find(dichVuId);
        if (dv == null) return NotFound();
        ViewBag.DichVu = dv;
        var ds = _db.BangGias.Include(b => b.LoaiDo).Include(b => b.DonViTinh)
                    .Where(b => b.MaDichVu == dichVuId)
                    .OrderBy(b => b.LoaiDo!.TenLoaiDo).ThenByDescending(b => b.NgayApDung).ToList();
        return View(ds);
    }

    private void NapForm(int dichVuId)
    {
        ViewBag.DichVu = _db.DichVus.Find(dichVuId);
        ViewBag.LoaiDos = _db.LoaiDos.Where(x => x.TrangThai).OrderBy(x => x.TenLoaiDo).ToList();
        ViewBag.DonViTinhs = _db.DonViTinhs.Where(x => x.TrangThai).OrderBy(x => x.TenDonViTinh).ToList();
    }

    private void KiemTra(BangGiaDichVu m)
    {
        if (m.NgayKetThuc != null && m.NgayKetThuc.Value.Date < m.NgayApDung.Date)
            ModelState.AddModelError("NgayKetThuc", "Ngày kết thúc phải sau hoặc bằng ngày áp dụng");
        if (m.TrangThai && ModelState.IsValid)
        {
            var tu = m.NgayApDung.Date;
            var den = m.NgayKetThuc?.Date ?? new DateTime(9999, 1, 1);
            bool chong = _db.BangGias.Any(b => b.MaBangGia != m.MaBangGia && b.MaDichVu == m.MaDichVu && b.MaLoaiDo == m.MaLoaiDo
                         && b.MaDonViTinh == m.MaDonViTinh && b.TrangThai
                         && b.NgayApDung <= den && (b.NgayKetThuc == null || b.NgayKetThuc >= tu));
            if (chong) ModelState.AddModelError("", "Khoảng thời gian áp dụng bị chồng lấn với một bảng giá khác của cùng dịch vụ, loại đồ và đơn vị tính.");
        }
    }

    [HttpGet]
    public IActionResult Create(int dichVuId)
    {
        if (_db.DichVus.Find(dichVuId) == null) return NotFound();
        NapForm(dichVuId);
        return View("Form", new BangGiaDichVu { MaDichVu = dichVuId });
    }

    [HttpPost]
    public IActionResult Create(BangGiaDichVu model)
    {
        KiemTra(model);
        if (!ModelState.IsValid) { NapForm(model.MaDichVu); return View("Form", model); }
        model.MaBangGia = 0;
        _db.BangGias.Add(model);
        _db.SaveChanges();
        ThongBao("Thêm bảng giá thành công");
        return RedirectToAction(nameof(Index), new { dichVuId = model.MaDichVu });
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var x = _db.BangGias.Find(id);
        if (x == null) return NotFound();
        NapForm(x.MaDichVu);
        return View("Form", x);
    }

    [HttpPost]
    public IActionResult Edit(BangGiaDichVu model)
    {
        var x = _db.BangGias.Find(model.MaBangGia);
        if (x == null) return NotFound();
        model.MaDichVu = x.MaDichVu;
        KiemTra(model);
        if (!ModelState.IsValid) { NapForm(x.MaDichVu); return View("Form", model); }
        x.MaLoaiDo = model.MaLoaiDo; x.MaDonViTinh = model.MaDonViTinh; x.DonGia = model.DonGia;
        x.NgayApDung = model.NgayApDung; x.NgayKetThuc = model.NgayKetThuc; x.TrangThai = model.TrangThai;
        _db.SaveChanges();
        ThongBao("Cập nhật bảng giá thành công");
        return RedirectToAction(nameof(Index), new { dichVuId = x.MaDichVu });
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var x = _db.BangGias.Find(id);
        if (x == null) return NotFound();
        int dv = x.MaDichVu;
        _db.BangGias.Remove(x);   // đơn cũ đã lưu đơn giá áp dụng riêng nên xóa an toàn
        _db.SaveChanges();
        ThongBao("Đã xóa dòng bảng giá");
        return RedirectToAction(nameof(Index), new { dichVuId = dv });
    }
}
