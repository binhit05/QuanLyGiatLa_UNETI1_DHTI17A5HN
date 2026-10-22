using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public static class TrangThaiDon
{
    public const string MoiTiepNhan = "Mới tiếp nhận";
    public const string DaXacNhan = "Đã xác nhận";
    public const string DangXuLy = "Đang xử lý";
    public const string ChoGiaoTra = "Chờ giao trả";
    public const string HoanThanh = "Hoàn thành";
    public const string DaHuy = "Đã hủy";
    public static readonly string[] TatCa = { MoiTiepNhan, DaXacNhan, DangXuLy, ChoGiaoTra, HoanThanh, DaHuy };
}

public class DonGiat
{
    [Key] public int MaDonGiat { get; set; }
    public int MaKhachHang { get; set; }
    public KhachHang? KhachHang { get; set; }
    public DateTime NgayTiepNhan { get; set; } = DateTime.Now;
    public DateTime NgayHenTra { get; set; }
    public int? MaNguoiTiepNhan { get; set; }
    public TaiKhoan? NguoiTiepNhan { get; set; }
    [StringLength(30)] public string TrangThai { get; set; } = TrangThaiDon.MoiTiepNhan;
    public decimal PhuPhi { get; set; }
    public decimal TongTienDuKien { get; set; }
    [StringLength(500)] public string? GhiChu { get; set; }

    public List<ChiTietDonGiat> ChiTiets { get; set; } = new();
    public List<PhanCongXuLy> PhanCongs { get; set; } = new();
    public List<CongDoanXuLy> CongDoans { get; set; } = new();
    public List<ThanhToan> ThanhToans { get; set; } = new();
    public List<KhieuNai> KhieuNais { get; set; } = new();
    public List<LichSuXuLy> LichSus { get; set; } = new();
    public GiaoTra? GiaoTra { get; set; }

    // Thuộc tính tính toán (không lưu DB). Cần Include ThanhToans trước khi dùng.
    [NotMapped] public decimal DaThanhToan => ThanhToans?.Sum(t => t.SoTien) ?? 0;
    [NotMapped] public decimal ConLai => TongTienDuKien - DaThanhToan;
    [NotMapped] public string TrangThaiThanhToan => DaThanhToan <= 0 ? "Chưa thanh toán" : (ConLai > 0 ? "Một phần" : "Đã thanh toán");
    [NotMapped] public bool QuaHan => NgayHenTra < DateTime.Now && TrangThai != TrangThaiDon.HoanThanh && TrangThai != TrangThaiDon.DaHuy;
    [NotMapped] public string TrangThaiHienThi => QuaHan ? "Quá hạn" : TrangThai;
}
