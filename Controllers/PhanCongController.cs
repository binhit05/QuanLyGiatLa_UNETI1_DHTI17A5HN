// Họ và tên: Lê Hoàng Anh
// Mã sinh viên: 23103100214
// Nội dung thực hiện: Phân công xử lý, cập nhật công đoạn, theo dõi tiến độ và đơn quá hẹn.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Filters;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers;

// Phân công & theo dõi xử lý: Admin xem/phân công tất cả, nhân viên xử lý chỉ thấy việc của mình
[PhanQuyen(VaiTroConst.Admin, VaiTroConst.NhanVienXuLy)]
public class PhanCongController : BaseController
{
    public PhanCongController(AppDbContext db) : base(db) { }

    private IQueryable<DonGiat> NapDon()
    {
        return _db.DonGiats.Include(d => d.KhachHang).Include(d => d.ChiTiets).ThenInclude(c => c.DichVu)
                 .Include(d => d.PhanCongs).ThenInclude(p => p.NhanVien).Include(d => d.CongDoans);
    }

    private bool LaViecCuaToi(DonGiat d) => d.PhanCongs.Any(p => p.NhanVien != null && p.NhanVien.MaTaiKhoan == MaNguoiDung);

    public IActionResult Index(string? q, int? nhanVien, string? congDoan, string? sort, int page = 1)
    {
        var now = DateTime.Now;
        var dangLam = NapDon().Where(d => d.TrangThai == TrangThaiDon.DaXacNhan || d.TrangThai == TrangThaiDon.DangXuLy).ToList();
        if (!LaAdmin) dangLam = dangLam.Where(LaViecCuaToi).ToList();

        ViewBag.ChoPhanCong = dangLam.Count(d => d.TrangThai == TrangThaiDon.DaXacNhan && !d.PhanCongs.Any());
        ViewBag.DangThucHien = dangLam.Count(d => d.TrangThai == TrangThaiDon.DangXuLy);
        ViewBag.SapDenHan = dangLam.Count(d => d.NgayHenTra >= now && d.NgayHenTra <= now.AddHours(24));
        ViewBag.QuaHan = dangLam.Count(d => d.QuaHan);

        var rows = new List<PhanCongRow>();
        foreach (var d in dangLam.Where(x => x.PhanCongs.Any()))
        {
            var cds = d.CongDoans.OrderBy(c => c.ThuTu).ToList();
            int xong = cds.Count(c => c.TrangThai == QuyTrinh.HoanThanh);
            var hienTai = cds.FirstOrDefault(c => c.TrangThai != QuyTrinh.HoanThanh);
            rows.Add(new PhanCongRow
            {
                Don = d,
                NhanVien = d.PhanCongs.First().NhanVien?.HoTen ?? "",
                TienDo = cds.Count > 0 ? xong * 100 / cds.Count : 0,
                CongDoanHienTai = hienTai != null ? hienTai.TenCongDoan : "Hoàn tất",
                Nguy = d.QuaHan || d.NgayHenTra <= now.AddHours(6)
            });
        }
        if (!string.IsNullOrWhiteSpace(q))
        {
            var s = q.Trim().ToLower();
            rows = rows.Where(r => UiHelper.MaDon(r.Don).ToLower().Contains(s) || (r.Don.KhachHang != null && r.Don.KhachHang.HoTen.ToLower().Contains(s))
                                   || r.NhanVien.ToLower().Contains(s)).ToList();
        }
        if (nhanVien != null) rows = rows.Where(r => r.Don.PhanCongs.Any(p => p.MaNhanVien == nhanVien)).ToList();
        if (!string.IsNullOrEmpty(congDoan)) rows = rows.Where(r => r.CongDoanHienTai == congDoan).ToList();
        rows = sort == "moi" ? rows.OrderByDescending(r => r.Don.NgayTiepNhan).ToList() : rows.OrderBy(r => r.Don.NgayHenTra).ToList();

        ViewBag.Filter = new FilterBarVM
        {
            Placeholder = "Tìm mã đơn, nhân viên...", Q = q, Sort = sort ?? "han",
            Selects = new List<SelectBox>
            {
                new SelectBox { Name = "nhanVien", AllText = "Tất cả nhân viên", Selected = nhanVien?.ToString(),
                    Opts = _db.NhanViens.Where(n => n.TaiKhoan!.VaiTro == VaiTroConst.NhanVienXuLy).OrderBy(n => n.HoTen).ToList().Select(n => new Opt(n.MaNhanVien.ToString(), n.HoTen)).ToList() },
                new SelectBox { Name = "congDoan", AllText = "Tất cả công đoạn", Selected = congDoan,
                    Opts = QuyTrinh.CongDoans.Select(c => new Opt(c, c)).ToList() }
            },
            Sorts = new List<Opt> { new Opt("han", "Hạn hoàn thành gần nhất"), new Opt("moi", "Đơn mới nhất") }
        };
        return View(PagedList<PhanCongRow>.Tao(rows.AsQueryable(), page, 6));
    }

    // ----- Phân công mới (Admin) -----
    private void NapForm()
    {
        var da = _db.PhanCongs.Select(p => p.MaDonGiat).ToList();
        var dons = _db.DonGiats.Include(d => d.KhachHang)
            .Where(d => (d.TrangThai == TrangThaiDon.DaXacNhan || d.TrangThai == TrangThaiDon.DangXuLy) && !da.Contains(d.MaDonGiat))
            .OrderBy(d => d.NgayHenTra).ToList();
        ViewBag.Dons = dons.Select(d => new Opt(d.MaDonGiat.ToString(), UiHelper.MaDon(d) + " — " + d.KhachHang!.HoTen + " (hẹn " + d.NgayHenTra.ToString("dd/MM HH:mm") + ")")).ToList();
        ViewBag.NhanViens = _db.NhanViens.Include(n => n.TaiKhoan)
            .Where(n => n.TrangThai && n.TaiKhoan!.VaiTro == VaiTroConst.NhanVienXuLy).OrderBy(n => n.HoTen).ToList();
    }

    [PhanQuyen(VaiTroConst.Admin)]
    [HttpGet]
    public IActionResult Create(int? donId)
    {
        NapForm();
        return View(new PhanCongVM { MaDonGiat = donId ?? 0 });
    }

    [PhanQuyen(VaiTroConst.Admin)]
    [HttpPost]
    public IActionResult Create(PhanCongVM vm)
    {
        var don = _db.DonGiats.Include(d => d.PhanCongs).FirstOrDefault(d => d.MaDonGiat == vm.MaDonGiat);
        var nv = _db.NhanViens.Include(n => n.TaiKhoan).FirstOrDefault(n => n.MaNhanVien == vm.MaNhanVien);
        if (ModelState.IsValid)
        {
            if (don == null) ModelState.AddModelError("MaDonGiat", "Đơn giặt không tồn tại");
            else if (don.TrangThai != TrangThaiDon.DaXacNhan && don.TrangThai != TrangThaiDon.DangXuLy) ModelState.AddModelError("MaDonGiat", "Chỉ phân công cho đơn đã xác nhận hoặc đang xử lý");
            else if (don.PhanCongs.Any()) ModelState.AddModelError("MaDonGiat", "Đơn này đã được phân công");
            if (nv == null || !nv.TrangThai || nv.TaiKhoan == null || nv.TaiKhoan.VaiTro != VaiTroConst.NhanVienXuLy)
                ModelState.AddModelError("MaNhanVien", "Nhân viên phải đang hoạt động và có vai trò xử lý");
        }
        if (!ModelState.IsValid) { NapForm(); return View(vm); }

        _db.PhanCongs.Add(new PhanCongXuLy { MaDonGiat = don!.MaDonGiat, MaNhanVien = nv!.MaNhanVien, NoiDungCongViec = vm.NoiDungCongViec, NgayPhanCong = DateTime.Now });
        for (int i = 0; i < QuyTrinh.CongDoans.Length; i++)
            _db.CongDoans.Add(new CongDoanXuLy { MaDonGiat = don.MaDonGiat, TenCongDoan = QuyTrinh.CongDoans[i], ThuTu = i + 1, MaNhanVien = nv.MaNhanVien });
        GhiLichSu(don.MaDonGiat, "Đã phân công xử lý cho " + nv.HoTen);
        _db.SaveChanges();
        ThongBao("Phân công thành công");
        return RedirectToAction(nameof(Index));
    }

    // ----- Cập nhật tiến độ -----
    public IActionResult CapNhat(int id)
    {
        var d = NapDon().FirstOrDefault(x => x.MaDonGiat == id);
        if (d == null) return NotFound();
        if (!LaAdmin && !LaViecCuaToi(d)) return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        ViewBag.NhanViens = _db.NhanViens.Include(n => n.TaiKhoan)
            .Where(n => n.TrangThai && n.TaiKhoan!.VaiTro == VaiTroConst.NhanVienXuLy).OrderBy(n => n.HoTen).ToList();
        return View(d);
    }

    [HttpPost]
    public IActionResult CapNhatCongDoan(int id, string hanhDong)
    {
        var cd = _db.CongDoans.Find(id);
        if (cd == null) return NotFound();
        var don = NapDon().FirstOrDefault(x => x.MaDonGiat == cd.MaDonGiat);
        if (don == null) return NotFound();
        if (!LaAdmin && !LaViecCuaToi(don)) return RedirectToAction("TuChoiTruyCap", "TaiKhoan");
        if (don.TrangThai != TrangThaiDon.DaXacNhan && don.TrangThai != TrangThaiDon.DangXuLy)
        { BaoLoi("Đơn không ở trạng thái xử lý nên không thể cập nhật công đoạn."); return RedirectToAction(nameof(CapNhat), new { id = don.MaDonGiat }); }

        var cacCd = don.CongDoans.OrderBy(c => c.ThuTu).ToList();
        var now = DateTime.Now;
        if (hanhDong == "batdau")
        {
            if (cd.TrangThai != QuyTrinh.ChuaBatDau) { BaoLoi("Công đoạn này đã bắt đầu."); return RedirectToAction(nameof(CapNhat), new { id = don.MaDonGiat }); }
            if (cacCd.Any(c => c.ThuTu < cd.ThuTu && c.TrangThai != QuyTrinh.HoanThanh)) { BaoLoi("Phải hoàn thành các công đoạn trước đó."); return RedirectToAction(nameof(CapNhat), new { id = don.MaDonGiat }); }
            if (cacCd.Any(c => c.TrangThai == QuyTrinh.DangThucHien)) { BaoLoi("Đang có công đoạn khác thực hiện, hãy hoàn thành trước."); return RedirectToAction(nameof(CapNhat), new { id = don.MaDonGiat }); }
            cd.TrangThai = QuyTrinh.DangThucHien; cd.ThoiGianBatDau = now;
            if (don.TrangThai == TrangThaiDon.DaXacNhan) { don.TrangThai = TrangThaiDon.DangXuLy; GhiLichSu(don.MaDonGiat, "Đơn chuyển sang Đang xử lý"); }
            GhiLichSu(don.MaDonGiat, "Đang thực hiện công đoạn " + cd.TenCongDoan.ToLower());
        }
        else if (hanhDong == "hoanthanh")
        {
            if (cd.TrangThai != QuyTrinh.DangThucHien) { BaoLoi("Chỉ hoàn thành được công đoạn đang thực hiện."); return RedirectToAction(nameof(CapNhat), new { id = don.MaDonGiat }); }
            cd.TrangThai = QuyTrinh.HoanThanh; cd.ThoiGianKetThuc = now;
            GhiLichSu(don.MaDonGiat, "Hoàn thành công đoạn " + cd.TenCongDoan.ToLower());
            if (cacCd.All(c => c.TrangThai == QuyTrinh.HoanThanh))
            {
                don.TrangThai = TrangThaiDon.ChoGiaoTra;
                GhiLichSu(don.MaDonGiat, "Hoàn tất xử lý, đơn chờ giao trả");
            }
        }
        else return BadRequest();

        // đồng bộ trạng thái phân công
        foreach (var p in don.PhanCongs)
            p.TrangThai = cacCd.All(c => c.TrangThai == QuyTrinh.HoanThanh) ? QuyTrinh.HoanThanh
                        : cacCd.Any(c => c.TrangThai != QuyTrinh.ChuaBatDau) ? QuyTrinh.DangThucHien : QuyTrinh.ChuaBatDau;
        _db.SaveChanges();
        ThongBao("Đã cập nhật công đoạn");
        return RedirectToAction(nameof(CapNhat), new { id = don.MaDonGiat });
    }

    [PhanQuyen(VaiTroConst.Admin)]
    [HttpPost]
    public IActionResult DoiNhanVien(int id, int maNhanVien)
    {
        var don = NapDon().FirstOrDefault(x => x.MaDonGiat == id);
        var nv = _db.NhanViens.Include(n => n.TaiKhoan).FirstOrDefault(n => n.MaNhanVien == maNhanVien);
        if (don == null || nv == null || !nv.TrangThai || nv.TaiKhoan == null || nv.TaiKhoan.VaiTro != VaiTroConst.NhanVienXuLy)
        { BaoLoi("Nhân viên không hợp lệ"); return RedirectToAction(nameof(CapNhat), new { id }); }
        foreach (var p in don.PhanCongs) p.MaNhanVien = nv.MaNhanVien;
        foreach (var c in don.CongDoans.Where(c => c.TrangThai != QuyTrinh.HoanThanh)) c.MaNhanVien = nv.MaNhanVien;
        GhiLichSu(id, "Phân công lại cho " + nv.HoTen);
        _db.SaveChanges();
        ThongBao("Đã đổi nhân viên phụ trách");
        return RedirectToAction(nameof(CapNhat), new { id });
    }
}
