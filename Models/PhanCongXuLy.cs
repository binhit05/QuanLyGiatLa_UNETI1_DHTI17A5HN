// Họ và tên: Lê Hoàng Anh
// Mã sinh viên: 23103100214
// Nội dung thực hiện: Khai báo entity PhanCongXuLy.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class PhanCongXuLy
{
    [Key] public int MaPhanCong { get; set; }
    public int MaDonGiat { get; set; }
    public DonGiat? DonGiat { get; set; }
    public int MaNhanVien { get; set; }
    public NhanVien? NhanVien { get; set; }
    public DateTime NgayPhanCong { get; set; } = DateTime.Now;
    [StringLength(300)] public string? NoiDungCongViec { get; set; }
    [StringLength(30)] public string TrangThai { get; set; } = "Chưa bắt đầu";
    [StringLength(300)] public string? GhiChu { get; set; }
}
