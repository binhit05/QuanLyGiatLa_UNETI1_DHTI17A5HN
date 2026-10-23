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

        var conThu = _db.DonGiats.Include(d => d.ThanhToans).Where(d => d.TrangThai != TrangThaiDon.DaHuy).ToList().Where(d => d.ConLai > 0).ToList();
        vm.ConPhaiThu = conThu.Sum(d => d.ConLai);
        vm.SoDonConThu = conThu.Count;

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
        var dauThang = new DateTime(today.Year, today.Month, 1);
        var luot = _db.ChiTietDonGiats.Where(c => c.DonGiat!.NgayTiepNhan >= dauThang && c.DonGiat.TrangThai != TrangThaiDon.DaHuy)
                      .GroupBy(c => c.DichVu!.TenDichVu).Select(g => new { Ten = g.Key, Dem = g.Count() }).ToList()
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
        loai = string.IsNullOrEmpty(loai) ? "tong-quan" : loai;

        var dons = _db.DonGiats.Include(d => d.KhachHang).Include(d => d.ChiTiets).ThenInclude(c => c.DichVu)
                      .Include(d => d.ThanhToans).Include(d => d.GiaoTra)
                      .Where(d => d.NgayTiepNhan >= tu && d.NgayTiepNhan < denEnd && d.TrangThai != TrangThaiDon.DaHuy).ToList();

        decimal doanhThu = DoanhThu(tu, denEnd);
        int soDon = dons.Count;
        decimal trungBinh = soDon > 0 ? dons.Sum(d => d.TongTienDuKien) / soDon : 0;
        var daGiao = dons.Where(d => d.GiaoTra != null).ToList();
        double tyLe = daGiao.Count > 0 ? daGiao.Count(d => d.GiaoTra!.NgayGiaoTra <= d.NgayHenTra) * 100.0 / daGiao.Count : 0;

        // so với kỳ trước
        int soNgay = (den - tu).Days + 1;
        var tuTruoc = tu.AddDays(-soNgay);
        decimal doanhThuTruoc = DoanhThu(tuTruoc, tu);
        int soDonTruoc = _db.DonGiats.Count(d => d.NgayTiepNhan >= tuTruoc && d.NgayTiepNhan < tu && d.TrangThai != TrangThaiDon.DaHuy);
        ViewBag.SoSanhDoanhThu = doanhThuTruoc > 0 ? Math.Round((doanhThu - doanhThuTruoc) * 100 / doanhThuTruoc, 1) : (decimal?)null;
        ViewBag.SoSanhDon = soDonTruoc > 0 ? Math.Round((soDon - soDonTruoc) * 100.0 / soDonTruoc, 1) : (double?)null;

        // doanh thu theo ngày (hoặc theo tháng nếu khoảng dài)
        var tts = _db.ThanhToans.Where(t => t.NgayThanhToan >= tu && t.NgayThanhToan < denEnd).ToList();
        var nhan = new List<string>(); var giaTri = new List<decimal>();
        if (soNgay <= 62)
        {
            var theoNgay = tts.GroupBy(t => t.NgayThanhToan.Date).ToDictionary(g => g.Key, g => g.Sum(x => x.SoTien));
            for (var n = tu; n <= den; n = n.AddDays(1))
            { nhan.Add(n.ToString("dd/MM")); giaTri.Add(Math.Round((theoNgay.ContainsKey(n) ? theoNgay[n] : 0) / 1000000m, 2)); }
        }
        else
        {
            foreach (var g in tts.GroupBy(t => new DateTime(t.NgayThanhToan.Year, t.NgayThanhToan.Month, 1)).OrderBy(g => g.Key))
            { nhan.Add(g.Key.ToString("MM/yyyy")); giaTri.Add(Math.Round(g.Sum(x => x.SoTien) / 1000000m, 2)); }
        }

        // doanh thu theo dịch vụ
        var theoDv = dons.SelectMany(d => d.ChiTiets).GroupBy(c => c.DichVu!.TenDichVu)
            .Select(g => new DongThongKe { Ten = g.Key, SoLuong = g.Count(), TongTien = g.Sum(c => c.ThanhTien) })
            .OrderByDescending(x => x.TongTien).ToList();
        decimal tongDv = theoDv.Sum(x => x.TongTien);
        foreach (var x in theoDv) x.TyLe = tongDv > 0 ? Math.Round(x.TongTien * 100 / tongDv, 1) : 0;
        var dvHienThi = theoDv.Take(4).ToList();
        if (theoDv.Count > 4)
            dvHienThi.Add(new DongThongKe { Ten = "Dịch vụ khác", SoLuong = theoDv.Skip(4).Sum(x => x.SoLuong), TongTien = theoDv.Skip(4).Sum(x => x.TongTien), TyLe = theoDv.Skip(4).Sum(x => x.TyLe) });

        // bảng chi tiết theo loại báo cáo
        var topKhach = dons.GroupBy(d => d.KhachHang!.HoTen)
            .Select(g => new DongThongKe { Ten = g.Key, SoLuong = g.Count(), TongTien = g.Sum(d => d.TongTienDuKien) })
            .OrderByDescending(x => x.TongTien).Take(5).ToList();

        ViewBag.Tu = tu; ViewBag.Den = den; ViewBag.Loai = loai;
        ViewBag.DoanhThu = doanhThu; ViewBag.SoDon = soDon; ViewBag.TrungBinh = trungBinh; ViewBag.TyLe = tyLe;
        ViewBag.NhanJson = JsonSerializer.Serialize(nhan); ViewBag.GiaTriJson = JsonSerializer.Serialize(giaTri);
        ViewBag.DvHienThi = dvHienThi; ViewBag.TheoDv = theoDv; ViewBag.TopKhach = topKhach;
        ViewBag.CongNo = dons.Where(d => d.ConLai > 0).OrderByDescending(d => d.ConLai).ToList();
        ViewBag.QuaHan = dons.Where(d => d.QuaHan).OrderBy(d => d.NgayHenTra).ToList();
        ViewBag.KyTruoc = tuTruoc.ToString("dd/MM") + " - " + tu.AddDays(-1).ToString("dd/MM");
        return View();
    }

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
