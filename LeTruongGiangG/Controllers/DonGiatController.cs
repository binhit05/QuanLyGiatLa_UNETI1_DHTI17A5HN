using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Filters;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers;

[PhanQuyen]
public class DonGiatController : BaseController
{
    public DonGiatController(AppDbContext db) : base(db) { }

    // ================= Danh sách =================
    public IActionResult Index(string? q, string? trangThai, int? dichVu, DateTime? ngay, string? tab, string? sort, int page = 1)
    {
        var now = DateTime.Now;
        var all = _db.DonGiats.AsQueryable();
        ViewBag.CTong = all.Count();
        ViewBag.CMoi = all.Count(d => d.TrangThai == TrangThaiDon.MoiTiepNhan);
        ViewBag.CDang = all.Count(d => d.TrangThai == TrangThaiDon.DangXuLy);
        ViewBag.CCho = all.Count(d => d.TrangThai == TrangThaiDon.ChoGiaoTra);
        ViewBag.CQuaHan = all.Count(d => d.NgayHenTra < now && d.TrangThai != TrangThaiDon.HoanThanh && d.TrangThai != TrangThaiDon.DaHuy);

        var query = all;
        switch (tab)
        {
            case "moi": query = query.Where(d => d.TrangThai == TrangThaiDon.MoiTiepNhan); break;
            case "dang": query = query.Where(d => d.TrangThai == TrangThaiDon.DangXuLy); break;
            case "cho": query = query.Where(d => d.TrangThai == TrangThaiDon.ChoGiaoTra); break;
            case "quahan": query = query.Where(d => d.NgayHenTra < now && d.TrangThai != TrangThaiDon.HoanThanh && d.TrangThai != TrangThaiDon.DaHuy); break;
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim(); var id = UiHelper.ThamSoMa(s, "DG-");
            query = query.Where(d => (id != null && d.MaDonGiat == id) || d.KhachHang!.HoTen.Contains(s) || d.KhachHang!.SoDienThoai.Contains(s));
        }
        if (trangThai == "Quá hạn") query = query.Where(d => d.NgayHenTra < now && d.TrangThai != TrangThaiDon.HoanThanh && d.TrangThai != TrangThaiDon.DaHuy);
        else if (!string.IsNullOrEmpty(trangThai)) query = query.Where(d => d.TrangThai == trangThai);
        if (dichVu != null) query = query.Where(d => d.ChiTiets.Any(c => c.MaDichVu == dichVu));
        if (ngay != null) { var n0 = ngay.Value.Date; var n1 = n0.AddDays(1); query = query.Where(d => d.NgayTiepNhan >= n0 && d.NgayTiepNhan < n1); }

        query = sort switch
        {
            "cu" => query.OrderBy(d => d.NgayTiepNhan),
            "hentra" => query.OrderBy(d => d.NgayHenTra),
            "tien" => query.OrderByDescending(d => d.TongTienDuKien),
            _ => query.OrderByDescending(d => d.NgayTiepNhan)
        };

        var ds = PagedList<DonGiat>.Tao(
            query.Include(d => d.KhachHang).Include(d => d.ChiTiets).ThenInclude(c => c.DichVu).Include(d => d.ThanhToans), page, 6);

        var tts = new List<Opt>();
        foreach (var t in TrangThaiDon.TatCa) tts.Add(new Opt(t, t));
        tts.Add(new Opt("Quá hạn", "Quá hạn"));
        ViewBag.Tab = tab ?? "";
        ViewBag.Filter = new FilterBarVM
        {
            Placeholder = "Mã đơn, tên hoặc SĐT khách hàng...",
            Q = q,
            Sort = sort ?? "moi",
            ShowDate = true,
            Ngay = ngay?.ToString("yyyy-MM-dd"),
            Selects = new List<SelectBox>
            {
                new SelectBox { Name = "trangThai", AllText = "Tất cả trạng thái", Selected = trangThai, Opts = tts },
                new SelectBox { Name = "dichVu", AllText = "Tất cả dịch vụ", Selected = dichVu?.ToString(),
                    Opts = _db.DichVus.OrderBy(x => x.TenDichVu).ToList().Select(x => new Opt(x.MaDichVu.ToString(), x.TenDichVu)).ToList() }
            },
            Sorts = new List<Opt> { new Opt("moi", "Mới nhất"), new Opt("cu", "Cũ nhất"), new Opt("hentra", "Hẹn trả sớm nhất"), new Opt("tien", "Tổng tiền cao nhất") },
            Hidden = string.IsNullOrEmpty(tab) ? new Dictionary<string, string>() : new Dictionary<string, string> { { "tab", tab } }
        };
        return View(ds);
    }

    // ================= Chi tiết =================
    public IActionResult Details(int id)
    {
        var d = _db.DonGiats
            .Include(x => x.KhachHang).Include(x => x.NguoiTiepNhan)
            .Include(x => x.ChiTiets).ThenInclude(c => c.DichVu)
            .Include(x => x.ChiTiets).ThenInclude(c => c.LoaiDo)
            .Include(x => x.ChiTiets).ThenInclude(c => c.DonViTinh)
            .Include(x => x.ThanhToans).Include(x => x.CongDoans)
            .Include(x => x.PhanCongs).ThenInclude(p => p.NhanVien)
            .Include(x => x.KhieuNais).Include(x => x.GiaoTra)
            .AsSplitQuery().FirstOrDefault(x => x.MaDonGiat == id);
        if (d == null) return NotFound();
        ViewBag.LichSu = _db.LichSuXuLys.Where(l => l.MaDonGiat == id).OrderByDescending(l => l.ThoiGian).ThenByDescending(l => l.MaLichSu).ToList();
        ViewBag.TiepTheo = QuyTrinh.TrangThaiTiepTheo(d.TrangThai);
        return View(d);
    }

    // ================= Tạo đơn =================
    private void NapForm(TaoDonVM vm)
    {
        var today = DateTime.Today;
        ViewBag.KhachHangs = _db.KhachHangs.Where(k => k.TrangThai).OrderBy(k => k.HoTen).ToList();
        ViewBag.NguoiTiepNhans = _db.TaiKhoans.Where(t => t.TrangThai && (t.VaiTro == VaiTroConst.Admin || t.VaiTro == VaiTroConst.NhanVienTiepNhan)).OrderBy(t => t.HoTen).ToList();

        var gia = _db.BangGias.Include(b => b.DonViTinh)
            .Where(b => b.TrangThai && b.NgayApDung <= today && (b.NgayKetThuc == null || b.NgayKetThuc >= today)
                        && b.DichVu!.TrangThai && b.LoaiDo!.TrangThai)
            .ToList()
            .Select(b => new { dv = b.MaDichVu, ld = b.MaLoaiDo, dvt = b.DonViTinh!.TenDonViTinh, gia = b.DonGia }).ToList();
        ViewBag.GiaJson = JsonSerializer.Serialize(gia);
        ViewBag.DichVuJson = JsonSerializer.Serialize(_db.DichVus.Where(x => x.TrangThai).OrderBy(x => x.TenDichVu).ToList().Select(x => new { id = x.MaDichVu, ten = x.TenDichVu, gio = x.ThoiGianXuLyDuKien }));
        ViewBag.LoaiDoJson = JsonSerializer.Serialize(_db.LoaiDos.Where(x => x.TrangThai).OrderBy(x => x.TenLoaiDo).ToList().Select(x => new { id = x.MaLoaiDo, ten = x.TenLoaiDo }));
        ViewBag.KhJson = JsonSerializer.Serialize(((List<KhachHang>)ViewBag.KhachHangs).Select(k => new { id = k.MaKhachHang, sdt = k.SoDienThoai, diaChi = k.DiaChi ?? "" }));
        var dong = vm.ChiTiets.Select(c => new { dv = c.MaDichVu, ld = c.MaLoaiDo, sl = c.SoLuong, tt = c.TinhTrang ?? "" }).ToList();
        ViewBag.RowsJson = JsonSerializer.Serialize(dong);
        ViewBag.MaDuKien = "DG-" + DateTime.Now.ToString("yyMMdd") + "-" + ((_db.DonGiats.Max(d => (int?)d.MaDonGiat) ?? 0) + 1).ToString("000");
    }

    [PhanQuyen(VaiTroConst.Admin, VaiTroConst.NhanVienTiepNhan)]
    [HttpGet]
    public IActionResult Create(int? khachHang)
    {
        var vm = new TaoDonVM { NgayTiepNhan = DateTime.Now, NgayHenTra = DateTime.Now.AddDays(1), MaNguoiTiepNhan = MaNguoiDung, MaKhachHang = khachHang };
        NapForm(vm);
        return View(vm);
    }

    private BangGiaDichVu? TimGia(int dv, int ld, DateTime ngay)
    {
        var n = ngay.Date;
        return _db.BangGias.Where(b => b.MaDichVu == dv && b.MaLoaiDo == ld && b.TrangThai && b.NgayApDung <= n
                                       && (b.NgayKetThuc == null || b.NgayKetThuc >= n))
                           .OrderByDescending(b => b.NgayApDung).FirstOrDefault();
    }

    [PhanQuyen(VaiTroConst.Admin, VaiTroConst.NhanVienTiepNhan)]
    [HttpPost]
    public IActionResult Create(TaoDonVM vm)
    {
        var dong = vm.ChiTiets.Where(c => c.MaDichVu > 0 || c.MaLoaiDo > 0).ToList();
        if (!dong.Any()) ModelState.AddModelError("", "Đơn giặt phải có ít nhất một dòng đồ giặt");
        if (vm.NgayHenTra <= vm.NgayTiepNhan) ModelState.AddModelError("NgayHenTra", "Ngày hẹn trả phải sau ngày tiếp nhận");
        if (vm.MaKhachHang != null && !_db.KhachHangs.Any(k => k.MaKhachHang == vm.MaKhachHang && k.TrangThai))
            ModelState.AddModelError("MaKhachHang", "Khách hàng không tồn tại hoặc đã ngừng hoạt động");

        var chiTiets = new List<ChiTietDonGiat>();
        int stt = 0;
        foreach (var c in dong)
        {
            stt++;
            if (c.MaDichVu <= 0 || c.MaLoaiDo <= 0) { ModelState.AddModelError("", "Dòng " + stt + ": chọn đủ dịch vụ và loại đồ"); continue; }
            if (c.SoLuong <= 0) { ModelState.AddModelError("", "Dòng " + stt + ": số lượng/khối lượng phải lớn hơn 0"); continue; }
            var g = TimGia(c.MaDichVu, c.MaLoaiDo, vm.NgayTiepNhan);
            if (g == null) { ModelState.AddModelError("", "Dòng " + stt + ": chưa có bảng giá hiệu lực cho dịch vụ và loại đồ này"); continue; }
            chiTiets.Add(new ChiTietDonGiat
            {
                MaDichVu = c.MaDichVu,
                MaLoaiDo = c.MaLoaiDo,
                MaDonViTinh = g.MaDonViTinh,
                SoLuongKhoiLuong = c.SoLuong,
                DonGiaApDung = g.DonGia,
                ThanhTien = Math.Round(c.SoLuong * g.DonGia, 0),
                TinhTrangLucNhan = c.TinhTrang
            });
        }
        if (!ModelState.IsValid) { NapForm(vm); return View(vm); }

        var don = new DonGiat
        {
            MaKhachHang = vm.MaKhachHang!.Value,
            NgayTiepNhan = vm.NgayTiepNhan,
            NgayHenTra = vm.NgayHenTra,
            MaNguoiTiepNhan = vm.MaNguoiTiepNhan ?? MaNguoiDung,
            GhiChu = vm.GhiChu,
            PhuPhi = vm.PhuPhi,
            TrangThai = TrangThaiDon.MoiTiepNhan,
            ChiTiets = chiTiets
        };
        don.TongTienDuKien = chiTiets.Sum(c => c.ThanhTien) + vm.PhuPhi;
        don.LichSus.Add(new LichSuXuLy { NoiDung = "Tiếp nhận đơn giặt", ThoiGian = DateTime.Now, NguoiThucHien = TenNguoiDung });
        _db.DonGiats.Add(don);
        _db.SaveChanges();
        ThongBao("Tạo đơn giặt " + UiHelper.MaDon(don) + " thành công");
        return RedirectToAction(nameof(Details), new { id = don.MaDonGiat });
    }

    // ================= Chỉnh sửa (thông tin chung) =================
    private void NapSua()
    {
        ViewBag.NguoiTiepNhans = _db.TaiKhoans.Where(t => t.TrangThai && (t.VaiTro == VaiTroConst.Admin || t.VaiTro == VaiTroConst.NhanVienTiepNhan)).OrderBy(t => t.HoTen).ToList();
    }

    [PhanQuyen(VaiTroConst.Admin, VaiTroConst.NhanVienTiepNhan)]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var d = _db.DonGiats.Include(x => x.KhachHang).Include(x => x.ChiTiets).ThenInclude(c => c.DichVu).FirstOrDefault(x => x.MaDonGiat == id);
        if (d == null) return NotFound();
        if (d.TrangThai == TrangThaiDon.HoanThanh || d.TrangThai == TrangThaiDon.DaHuy)
        { BaoLoi("Đơn đã hoàn thành hoặc đã hủy nên không thể chỉnh sửa"); return RedirectToAction(nameof(Details), new { id }); }
        NapSua();
        ViewBag.Don = d;
        return View(new SuaDonVM { MaDonGiat = d.MaDonGiat, NgayHenTra = d.NgayHenTra, MaNguoiTiepNhan = d.MaNguoiTiepNhan, GhiChu = d.GhiChu, PhuPhi = d.PhuPhi });
    }

    [PhanQuyen(VaiTroConst.Admin, VaiTroConst.NhanVienTiepNhan)]
    [HttpPost]
    public IActionResult Edit(SuaDonVM vm)
    {
        var d = _db.DonGiats.Include(x => x.KhachHang).Include(x => x.ChiTiets).ThenInclude(c => c.DichVu).FirstOrDefault(x => x.MaDonGiat == vm.MaDonGiat);
        if (d == null) return NotFound();
        if (d.TrangThai == TrangThaiDon.HoanThanh || d.TrangThai == TrangThaiDon.DaHuy)
        { BaoLoi("Đơn đã hoàn thành hoặc đã hủy nên không thể chỉnh sửa"); return RedirectToAction(nameof(Details), new { id = d.MaDonGiat }); }
        if (vm.NgayHenTra <= d.NgayTiepNhan) ModelState.AddModelError("NgayHenTra", "Ngày hẹn trả phải sau ngày tiếp nhận");
        if (!ModelState.IsValid) { NapSua(); ViewBag.Don = d; return View(vm); }

        d.NgayHenTra = vm.NgayHenTra; d.MaNguoiTiepNhan = vm.MaNguoiTiepNhan; d.GhiChu = vm.GhiChu; d.PhuPhi = vm.PhuPhi;
        d.TongTienDuKien = d.ChiTiets.Sum(c => c.ThanhTien) + vm.PhuPhi;
        GhiLichSu(d.MaDonGiat, "Cập nhật thông tin đơn giặt");
        _db.SaveChanges();
        ThongBao("Cập nhật đơn giặt thành công");
        return RedirectToAction(nameof(Details), new { id = d.MaDonGiat });
    }

    // ================= Xóa =================
    [PhanQuyen(VaiTroConst.Admin, VaiTroConst.NhanVienTiepNhan)]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var d = _db.DonGiats.Include(x => x.ThanhToans).Include(x => x.PhanCongs).Include(x => x.KhieuNais)
                  .Include(x => x.ChiTiets).Include(x => x.LichSus).FirstOrDefault(x => x.MaDonGiat == id);
        if (d == null) return NotFound();
        if (d.TrangThai != TrangThaiDon.MoiTiepNhan && d.TrangThai != TrangThaiDon.DaHuy)
        { BaoLoi("Chỉ được xóa đơn ở trạng thái Mới tiếp nhận hoặc Đã hủy. Với đơn khác hãy chuyển sang Đã hủy."); return RedirectToAction(nameof(Index)); }
        if (d.ThanhToans.Any() || d.PhanCongs.Any() || d.KhieuNais.Any())
        { BaoLoi("Đơn đã phát sinh thanh toán, phân công hoặc khiếu nại nên không thể xóa."); return RedirectToAction(nameof(Index)); }
        _db.LichSuXuLys.RemoveRange(d.LichSus);
        _db.ChiTietDonGiats.RemoveRange(d.ChiTiets);
        _db.DonGiats.Remove(d);
        _db.SaveChanges();
        ThongBao("Đã xóa đơn giặt");
        return RedirectToAction(nameof(Index));
    }

    // ================= Cập nhật trạng thái =================
    [PhanQuyen(VaiTroConst.Admin, VaiTroConst.NhanVienTiepNhan)]
    [HttpPost]
    public IActionResult CapNhatTrangThai(int id, string trangThai)
    {
        var d = _db.DonGiats.Include(x => x.PhanCongs).Include(x => x.CongDoans).Include(x => x.ThanhToans).FirstOrDefault(x => x.MaDonGiat == id);
        if (d == null) return NotFound();
        if (!QuyTrinh.TrangThaiTiepTheo(d.TrangThai).Contains(trangThai))
        { BaoLoi("Không thể chuyển từ \"" + d.TrangThai + "\" sang \"" + trangThai + "\""); return RedirectToAction(nameof(Details), new { id }); }
        if (trangThai == TrangThaiDon.DangXuLy && !d.PhanCongs.Any())
        { BaoLoi("Đơn chưa được phân công xử lý. Hãy phân công nhân viên trước."); return RedirectToAction(nameof(Details), new { id }); }
        if (trangThai == TrangThaiDon.ChoGiaoTra && (d.CongDoans.Count == 0 || d.CongDoans.Any(c => c.TrangThai != QuyTrinh.HoanThanh)))
        { BaoLoi("Chưa hoàn thành đủ các công đoạn xử lý nên chưa thể chuyển sang Chờ giao trả."); return RedirectToAction(nameof(Details), new { id }); }
        if (trangThai == TrangThaiDon.DaHuy && d.ThanhToans.Any())
        { BaoLoi("Đơn đã có thanh toán nên không thể hủy. Hãy xử lý hoàn tiền/điều chỉnh thanh toán trước."); return RedirectToAction(nameof(Details), new { id }); }

        var cu = d.TrangThai;
        d.TrangThai = trangThai;
        GhiLichSu(id, "Chuyển trạng thái đơn: " + cu + " → " + trangThai);
        _db.SaveChanges();
        ThongBao("Đã cập nhật trạng thái đơn");
        return RedirectToAction(nameof(Details), new { id });
    }
}
