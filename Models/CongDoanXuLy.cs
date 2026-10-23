// Họ và tên: Lê Hoàng Anh
// Mã sinh viên: 23103100214
// Nội dung thực hiện: Khai báo entity CongDoanXuLy.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class CongDoanXuLy
{
    [Key] public int MaCongDoan { get; set; }
    public int MaDonGiat { get; set; }
    public DonGiat? DonGiat { get; set; }
    [Required, StringLength(50)] public string TenCongDoan { get; set; } = "";
    public int ThuTu { get; set; }
    public DateTime? ThoiGianBatDau { get; set; }
    public DateTime? ThoiGianKetThuc { get; set; }
    public int? MaNhanVien { get; set; }
    public NhanVien? NhanVien { get; set; }
    [StringLength(30)] public string TrangThai { get; set; } = "Chưa bắt đầu";
    [StringLength(300)] public string? GhiChu { get; set; }
}
