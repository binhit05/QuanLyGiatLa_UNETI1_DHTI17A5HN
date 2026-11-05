// Họ và tên: Lê Duy Khánh
// Mã sinh viên: 23103100216
// Nội dung thực hiện: Dashboard và thống kê báo cáo bằng LINQ (GroupBy theo khoảng ngày), xuất CSV.
using System.Text;
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
public class ThongKeController : BaseController
{
    public ThongKeController(AppDbContext db) : base(db) { }

    // ================= Tổng quan (dashboard) =================
    public IActionResult Index()
    {
        var now = DateTime.Now; var today = now.Date; var homQua = today.AddDays(-1); var ngayMai = today.AddDays(1);
        var vm = new DashboardVM();

        vm.DonHomNay = _db.DonGiats.Count(d => d.NgayTiepNhan >= today && d.NgayTiepNhan < ngayMai);
        vm.DonHomQua = _db.DonGiats.Count(d => d.NgayTiepNhan >= homQua && d.NgayTiepNhan < today);
        vm.DangXuLy = _db.DonGiats.Count(d => d.TrangThai == TrangThaiDon.DangXuLy);
        var sau24 = now.AddHours(24);
        vm.SapDenHan = _db.DonGiats.Count(d => d.TrangThai == TrangThaiDon.DangXuLy && d.NgayHenTra >= now && d.NgayHenTra <= sau24);
        vm.ChoGiaoTra = _db.DonGiats.Count(d => d.TrangThai == TrangThaiDon.ChoGiaoTra);
        vm.QuaHan = _db.DonGiats.Count(d => d.NgayHenTra < now && d.TrangThai != TrangThaiDon.HoanThanh && d.TrangThai != TrangThaiDon.DaHuy);
        vm.KhieuNaiDangXuLy = _db.KhieuNais.Count(k => k.TrangThai != "Đã giải quyết");
        vm.KhieuNaiCao = _db.KhieuNais.Count(k => k.TrangThai != "Đã giải quyết" && k.MucDo == "Cao");
        vm.DoanhThuHomNay = _db.ThanhToans.Where(t => t.NgayThanhToan >= today && t.NgayThanhToan < ngayMai).Sum(t => (decimal?)t.SoTien) ?? 0;
        vm.DoanhThuHomQua = _db.ThanhToans.Where(t => t.NgayThanhToan >= homQua && t.NgayThanhToan < today).Sum(t => (decimal?)t.SoTien) ?? 0;

        // Doanh thu đã thu theo tháng (tháng này và tháng trước để so sánh)
        var dauThang = new DateTime(today.Year, today.Month, 1);
        vm.DoanhThuThangNay = DoanhThu(dauThang, dauThang.AddMonths(1));
        vm.DoanhThuThangTruoc = DoanhThu(dauThang.AddMonths(-1), dauThang);

        // Tổng còn phải thu = Σ (TongTienDuKien - tổng đã thanh toán) của các đơn còn nợ, tính trong SQL (không tải đơn về bộ nhớ)
        var conLaiTheoDon = _db.DonGiats.Where(d => d.TrangThai != TrangThaiDon.DaHuy)
                               .Select(d => d.TongTienDuKien - d.ThanhToans.Sum(t => t.SoTien))
                               .Where(conLai => conLai > 0);
        vm.SoDonConThu = conLaiTheoDon.Count();
        vm.ConPhaiThu = conLaiTheoDon.Sum(conLai => (decimal?)conLai) ?? 0;


        // doanh thu 30 ngày gần nhất (biểu đồ có thể chọn 7 hoặc 30 ngày)
        var tu30 = today.AddDays(-29);
        var tt = _db.ThanhToans.Where(t => t.NgayThanhToan >= tu30 && t.NgayThanhToan < ngayMai).ToList()
                    .GroupBy(t => t.NgayThanhToan.Date).ToDictionary(g => g.Key, g => g.Sum(x => x.SoTien));
        for (int i = 29; i >= 0; i--)
        {
            var ngay = today.AddDays(-i);
            decimal v = tt.ContainsKey(ngay) ? tt[ngay] : 0;
            vm.Nhan30.Add(ngay.ToString("dd/MM"));
            vm.Gia30.Add(Math.Round(v / 1000000m, 2));
            if (i < 7) { vm.Nhan.Add(ngay.ToString("dd/MM")); vm.Gia7.Add(Math.Round(v / 1000000m, 2)); }
        }

        // dịch vụ phổ biến trong tháng
        var luot = _db.ChiTietDonGiats.Where(c => c.DonGiat!.NgayTiepNhan >= dauThang && c.DonGiat.TrangThai != TrangThaiDon.DaHuy)
                      .GroupBy(c => c.DichVu!.TenDichVu).Select(g => new { Ten = g.Key, Dem = g.Count() })
                      .OrderByDescending(x => x.Dem).ToList();
        int tong = luot.Sum(x => x.Dem);
        if (tong > 0)
        {
            foreach (var x in luot.Take(4)) vm.DichVuPhoBien.Add(new DichVuTyLe { Ten = x.Ten, TyLe = (int)Math.Round(x.Dem * 100.0 / tong) });
            int khac = luot.Skip(4).Sum(x => x.Dem);
            if (khac > 0) vm.DichVuPhoBien.Add(new DichVuTyLe { Ten = "Khác", TyLe = (int)Math.Round(khac * 100.0 / tong) });
        }

        vm.DonGanDay = _db.DonGiats.Include(d => d.KhachHang).Include(d => d.ChiTiets).ThenInclude(c => c.DichVu)
                          .OrderByDescending(d => d.NgayTiepNhan).Take(5).ToList();
        return View(vm);
    }

    // ================= Thống kê báo cáo (Admin) =================
    private (DateTime tu, DateTime den) KhoangNgay(DateTime? tuNgay, DateTime? denNgay)
    {
        var den = (denNgay ?? DateTime.Today).Date;
        var tu = (tuNgay ?? new DateTime(den.Year, den.Month, 1)).Date;
        if (tu > den) { var tam = tu; tu = den; den = tam; }
        return (tu, den);
    }

    private decimal DoanhThu(DateTime tu, DateTime denExclusive) =>
        _db.ThanhToans.Where(t => t.NgayThanhToan >= tu && t.NgayThanhToan < denExclusive).Sum(t => (decimal?)t.SoTien) ?? 0;

    [PhanQuyen(VaiTroConst.Admin)]
    public IActionResult BaoCao(DateTime? tuNgay, DateTime? denNgay, string? loai)
    {
        var (tu, den) = KhoangNgay(tuNgay, denNgay);
        var denEnd = den.AddDays(1);
        var now = DateTime.Now;
        loai = string.IsNullOrEmpty(loai) ? "tong-quan" : loai;

        // Đơn tiếp nhận trong kỳ (trừ đơn hủy). Đây là IQueryable: chưa chạy SQL cho tới khi gọi Count/Sum/ToList.
        var donKy = _db.DonGiats.Where(d => d.NgayTiepNhan >= tu && d.NgayTiepNhan < denEnd && d.TrangThai != TrangThaiDon.DaHuy);

        // ----- Chỉ số tổng quan (mỗi chỉ số là 1 câu truy vấn tổng hợp, không tải danh sách đơn) -----
        decimal doanhThu = DoanhThu(tu, denEnd);
        int soDon = donKy.Count();
        decimal tongGiaTri = donKy.Sum(d => (decimal?)d.TongTienDuKien) ?? 0;
        decimal trungBinh = soDon > 0 ? tongGiaTri / soDon : 0;
        var daGiao = donKy.Where(d => d.GiaoTra != null);
        int soDaGiao = daGiao.Count();
        int soDungHan = daGiao.Count(d => d.GiaoTra!.NgayGiaoTra <= d.NgayHenTra);
        double tyLe = soDaGiao > 0 ? soDungHan * 100.0 / soDaGiao : 0;

        // So với kỳ trước
        int soNgay = (den - tu).Days + 1;
        var tuTruoc = tu.AddDays(-soNgay);
        decimal doanhThuTruoc = DoanhThu(tuTruoc, tu);
        int soDonTruoc = _db.DonGiats.Count(d => d.NgayTiepNhan >= tuTruoc && d.NgayTiepNhan < tu && d.TrangThai != TrangThaiDon.DaHuy);
        ViewBag.SoSanhDoanhThu = doanhThuTruoc > 0 ? Math.Round((doanhThu - doanhThuTruoc) * 100 / doanhThuTruoc, 1) : (decimal?)null;
        ViewBag.SoSanhDon = soDonTruoc > 0 ? Math.Round((soDon - soDonTruoc) * 100.0 / soDonTruoc, 1) : (double?)null;

        // ----- Doanh thu đã thu theo ngày (hoặc theo tháng nếu khoảng dài): GroupBy chạy trong SQL -----
        var thanhToanKy = _db.ThanhToans.Where(t => t.NgayThanhToan >= tu && t.NgayThanhToan < denEnd);
        var nhan = new List<string>(); var giaTri = new List<decimal>();
        if (soNgay <= 62)
        {
            var theoNgay = thanhToanKy.GroupBy(t => t.NgayThanhToan.Date).Select(g => new { Ngay = g.Key, Tong = g.Sum(x => x.SoTien) })
                                      .ToList().ToDictionary(x => x.Ngay, x => x.Tong);
            for (var n = tu; n <= den; n = n.AddDays(1))
            { nhan.Add(n.ToString("dd/MM")); giaTri.Add(Math.Round((theoNgay.ContainsKey(n) ? theoNgay[n] : 0) / 1000000m, 2)); }
        }
        else
        {
            var theoThang = thanhToanKy.GroupBy(t => new { t.NgayThanhToan.Year, t.NgayThanhToan.Month })
                                       .Select(g => new { g.Key.Year, g.Key.Month, Tong = g.Sum(x => x.SoTien) })
                                       .OrderBy(x => x.Year).ThenBy(x => x.Month).ToList();
            foreach (var g in theoThang)
            { nhan.Add(g.Month.ToString("00") + "/" + g.Year); giaTri.Add(Math.Round(g.Tong / 1000000m, 2)); }
        }

        // ----- Doanh thu (giá trị đơn) theo dịch vụ: GroupBy theo tên dịch vụ trên các dòng chi tiết của đơn trong kỳ -----
        var theoDv = _db.ChiTietDonGiats
            .Where(c => c.DonGiat!.NgayTiepNhan >= tu && c.DonGiat.NgayTiepNhan < denEnd && c.DonGiat.TrangThai != TrangThaiDon.DaHuy)
            .GroupBy(c => c.DichVu!.TenDichVu)
            .Select(g => new { Ten = g.Key, SoLuong = g.Count(), TongTien = g.Sum(c => c.ThanhTien) })
            .OrderByDescending(x => x.TongTien).ToList()
            .Select(x => new DongThongKe { Ten = x.Ten, SoLuong = x.SoLuong, TongTien = x.TongTien }).ToList();
        decimal tongDv = theoDv.Sum(x => x.TongTien);
        foreach (var x in theoDv) x.TyLe = tongDv > 0 ? Math.Round(x.TongTien * 100 / tongDv, 1) : 0;
        var dvHienThi = theoDv.Take(4).ToList();
        if (theoDv.Count > 4)
            dvHienThi.Add(new DongThongKe { Ten = "Dịch vụ khác", SoLuong = theoDv.Skip(4).Sum(x => x.SoLuong), TongTien = theoDv.Skip(4).Sum(x => x.TongTien), TyLe = theoDv.Skip(4).Sum(x => x.TyLe) });

        // ----- Khách hàng có số đơn nhiều nhất (GroupBy theo MaKhachHang, sắp xếp theo số đơn rồi tới tổng tiền) -----
        var topKhach = donKy.GroupBy(d => new { d.MaKhachHang, d.KhachHang!.HoTen })
            .Select(g => new { g.Key.HoTen, SoDon = g.Count(), TongTien = g.Sum(d => d.TongTienDuKien) })
            .OrderByDescending(x => x.SoDon).ThenByDescending(x => x.TongTien).Take(5).ToList()
            .Select(x => new DongThongKe { Ten = x.HoTen, SoLuong = x.SoDon, TongTien = x.TongTien }).ToList();

        ViewBag.Tu = tu; ViewBag.Den = den; ViewBag.Loai = loai;
        ViewBag.DoanhThu = doanhThu; ViewBag.SoDon = soDon; ViewBag.TrungBinh = trungBinh; ViewBag.TyLe = tyLe;
        ViewBag.NhanJson = JsonSerializer.Serialize(nhan); ViewBag.GiaTriJson = JsonSerializer.Serialize(giaTri);
        ViewBag.DvHienThi = dvHienThi; ViewBag.TheoDv = theoDv; ViewBag.TopKhach = topKhach;
        ViewBag.KyTruoc = tuTruoc.ToString("dd/MM") + " - " + tu.AddDays(-1).ToString("dd/MM");

        // Giá trị mặc định để View luôn ép kiểu được; chỉ truy vấn bảng chi tiết của loại báo cáo đang xem
        ViewBag.CongNo = new List<DonGiat>(); ViewBag.QuaHan = new List<DonGiat>();
        ViewBag.TheoTrangThai = new List<DongThongKe>(); ViewBag.TheoLoaiDo = new List<DongLoaiDo>();
        ViewBag.QuaHanTheoThang = new List<DongThang>(); ViewBag.TheoNhanVien = new List<DongNhanVien>();
        ViewBag.KhieuNaiTheoDv = new List<DongKhieuNai>();

        switch (loai)
        {
            case "cong-no":
                // Đơn trong kỳ còn nợ: lọc và sắp xếp theo số còn lại ngay trong SQL
                ViewBag.CongNo = donKy.Where(d => d.TongTienDuKien - d.ThanhToans.Sum(t => t.SoTien) > 0)
                    .OrderByDescending(d => d.TongTienDuKien - d.ThanhToans.Sum(t => t.SoTien))
                    .Include(d => d.KhachHang).Include(d => d.ThanhToans).ToList();
                break;

            case "qua-han":
                ViewBag.QuaHan = donKy.Where(d => d.NgayHenTra < now && d.TrangThai != TrangThaiDon.HoanThanh)
                    .OrderBy(d => d.NgayHenTra).Include(d => d.KhachHang).ToList();
                break;

            case "trang-thai":
                ViewBag.TheoTrangThai = ThongKeTheoTrangThai(tu, denEnd);
                break;

            case "loai-do":
                ViewBag.TheoLoaiDo = ThongKeTheoLoaiDo(tu, denEnd);
                break;

            case "qua-han-thang":
                ViewBag.QuaHanTheoThang = ThongKeQuaHanTheoThang(tu, denEnd, now);
                break;

            case "nhan-vien":
                ViewBag.TheoNhanVien = ThongKeTheoNhanVien(tu, denEnd);
                break;

            case "khieu-nai":
                ViewBag.KhieuNaiTheoDv = ThongKeKhieuNaiTheoDichVu(tu, denEnd);
                break;
        }
        return View();
    }
    // ================= Các thống kê LINQ theo khoảng ngày =================

    // 1) Số đơn theo trạng thái: GroupBy TrangThai, sau đó bổ sung các trạng thái chưa có đơn (số lượng 0)
    private List<DongThongKe> ThongKeTheoTrangThai(DateTime tu, DateTime denEnd)
    {
        var raw = _db.DonGiats.Where(d => d.NgayTiepNhan >= tu && d.NgayTiepNhan < denEnd)
            .GroupBy(d => d.TrangThai)
            .Select(g => new { TrangThai = g.Key, SoDon = g.Count(), TongTien = g.Sum(d => d.TongTienDuKien) }).ToList();
        int tong = raw.Sum(x => x.SoDon);
        return TrangThaiDon.TatCa.Select(t =>
        {
            var x = raw.FirstOrDefault(r => r.TrangThai == t);
            int n = x?.SoDon ?? 0;
            return new DongThongKe { Ten = t, SoLuong = n, TongTien = x?.TongTien ?? 0, TyLe = tong > 0 ? Math.Round(n * 100m / tong, 1) : 0 };
        }).ToList();
    }
    // 2) Số lượng/khối lượng theo loại đồ: GroupBy (loại đồ, đơn vị tính) vì kg/chiếc/bộ không cộng chung được
    private List<DongLoaiDo> ThongKeTheoLoaiDo(DateTime tu, DateTime denEnd) =>
        _db.ChiTietDonGiats
            .Where(c => c.DonGiat!.NgayTiepNhan >= tu && c.DonGiat.NgayTiepNhan < denEnd && c.DonGiat.TrangThai != TrangThaiDon.DaHuy)
            .GroupBy(c => new { c.LoaiDo!.TenLoaiDo, c.DonViTinh!.TenDonViTinh })
            .Select(g => new { g.Key.TenLoaiDo, g.Key.TenDonViTinh, SoDong = g.Count(), Tong = g.Sum(c => c.SoLuongKhoiLuong) })
            .OrderBy(x => x.TenLoaiDo).ThenBy(x => x.TenDonViTinh).ToList()
            .Select(x => new DongLoaiDo { TenLoaiDo = x.TenLoaiDo, TenDonViTinh = x.TenDonViTinh, SoDong = x.SoDong, TongSoLuongKhoiLuong = x.Tong }).ToList();

    // 3) Số đơn quá hẹn theo tháng (tháng của NgayHenTra, các đơn có hẹn trả nằm trong kỳ).
    //    Đơn quá hẹn = đã giao trả nhưng NgayGiaoTra > NgayHenTra, hoặc chưa giao/hoàn thành mà NgayHenTra < hiện tại.
    private List<DongThang> ThongKeQuaHanTheoThang(DateTime tu, DateTime denEnd, DateTime now)
    {
        var raw = _db.DonGiats.Where(d => d.NgayHenTra >= tu && d.NgayHenTra < denEnd && d.TrangThai != TrangThaiDon.DaHuy)
            .Select(d => new
            {
                d.NgayHenTra,
                QuaHen = d.GiaoTra != null ? d.GiaoTra.NgayGiaoTra > d.NgayHenTra
                                           : (d.NgayHenTra < now && d.TrangThai != TrangThaiDon.HoanThanh)
            })
            .GroupBy(x => new { x.NgayHenTra.Year, x.NgayHenTra.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, TongDon = g.Count(), SoQuaHen = g.Count(x => x.QuaHen) })
            .OrderBy(x => x.Year).ThenBy(x => x.Month).ToList();
        return raw.Select(x => new DongThang
        {
            Thang = x.Month.ToString("00") + "/" + x.Year,
            TongDon = x.TongDon,
            SoQuaHen = x.SoQuaHen,
            TyLe = x.TongDon > 0 ? Math.Round(x.SoQuaHen * 100m / x.TongDon, 1) : 0
        }).ToList();
    }
    // 4) Số công việc hoàn thành theo nhân viên: phân công hoàn thành (theo NgayPhanCong) và công đoạn hoàn thành (theo ThoiGianKetThuc)
    private List<DongNhanVien> ThongKeTheoNhanVien(DateTime tu, DateTime denEnd)
    {
        var phanCong = _db.PhanCongs.Where(p => p.TrangThai == QuyTrinh.HoanThanh && p.NgayPhanCong >= tu && p.NgayPhanCong < denEnd)
            .GroupBy(p => p.MaNhanVien).Select(g => new { Ma = g.Key, Dem = g.Count() }).ToList();
        var congDoan = _db.CongDoans.Where(c => c.TrangThai == QuyTrinh.HoanThanh && c.MaNhanVien != null
                                                && c.ThoiGianKetThuc >= tu && c.ThoiGianKetThuc < denEnd)
            .GroupBy(c => c.MaNhanVien!.Value).Select(g => new { Ma = g.Key, Dem = g.Count() }).ToList();

        var ids = phanCong.Select(x => x.Ma).Union(congDoan.Select(x => x.Ma)).ToList();
        var ten = _db.NhanViens.Where(n => ids.Contains(n.MaNhanVien)).ToDictionary(n => n.MaNhanVien, n => n.HoTen);
        return ids.Select(id => new DongNhanVien
        {
            MaNhanVien = id,
            HoTen = ten.ContainsKey(id) ? ten[id] : "(không rõ)",
            SoPhanCongHoanThanh = phanCong.FirstOrDefault(x => x.Ma == id)?.Dem ?? 0,
            SoCongDoanHoanThanh = congDoan.FirstOrDefault(x => x.Ma == id)?.Dem ?? 0
        }).OrderByDescending(x => x.SoPhanCongHoanThanh + x.SoCongDoanHoanThanh).ToList();
    }
    // 5) Tỷ lệ khiếu nại theo dịch vụ = số khiếu nại của các đơn có dịch vụ đó / số đơn có dịch vụ đó (đơn tiếp nhận trong kỳ)
    private List<DongKhieuNai> ThongKeKhieuNaiTheoDichVu(DateTime tu, DateTime denEnd) =>
        _db.DichVus.Select(dv => new
        {
            dv.TenDichVu,
            SoDon = _db.DonGiats.Count(d => d.NgayTiepNhan >= tu && d.NgayTiepNhan < denEnd && d.TrangThai != TrangThaiDon.DaHuy
                                            && d.ChiTiets.Any(c => c.MaDichVu == dv.MaDichVu)),
            SoKhieuNai = _db.KhieuNais.Count(k => k.DonGiat!.NgayTiepNhan >= tu && k.DonGiat.NgayTiepNhan < denEnd && k.DonGiat.TrangThai != TrangThaiDon.DaHuy
                                                  && k.DonGiat.ChiTiets.Any(c => c.MaDichVu == dv.MaDichVu))
        }).ToList()
        .Where(x => x.SoDon > 0)
        .Select(x => new DongKhieuNai
        {
            TenDichVu = x.TenDichVu,
            SoDon = x.SoDon,
            SoKhieuNai = x.SoKhieuNai,
            TyLe = Math.Round(x.SoKhieuNai * 100m / x.SoDon, 1)
        }).OrderByDescending(x => x.TyLe).ThenByDescending(x => x.SoKhieuNai).ToList();




    [PhanQuyen(VaiTroConst.Admin)]
    public IActionResult XuatBaoCao(DateTime? tuNgay, DateTime? denNgay)
    {
        var (tu, den) = KhoangNgay(tuNgay, denNgay);
        var denEnd = den.AddDays(1);
        var dons = _db.DonGiats.Include(d => d.KhachHang).Include(d => d.ThanhToans)
                      .Where(d => d.NgayTiepNhan >= tu && d.NgayTiepNhan < denEnd).OrderBy(d => d.NgayTiepNhan).ToList();
        string Q(string? s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";
        var sb = new StringBuilder();
        sb.AppendLine("Mã đơn,Khách hàng,Số điện thoại,Ngày tiếp nhận,Hẹn trả,Trạng thái,Tổng tiền,Đã thanh toán,Còn lại");
        foreach (var d in dons)
            sb.AppendLine(string.Join(",", Q(UiHelper.MaDon(d)), Q(d.KhachHang!.HoTen), Q(d.KhachHang.SoDienThoai), Q(d.NgayTiepNhan.ToString("dd/MM/yyyy HH:mm")),
                Q(d.NgayHenTra.ToString("dd/MM/yyyy HH:mm")), Q(d.TrangThaiHienThi), d.TongTienDuKien.ToString("0"), d.DaThanhToan.ToString("0"), d.ConLai.ToString("0")));
        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        return File(bytes, "text/csv", "bao-cao-" + tu.ToString("yyyyMMdd") + "-" + den.ToString("yyyyMMdd") + ".csv");
    }
}
