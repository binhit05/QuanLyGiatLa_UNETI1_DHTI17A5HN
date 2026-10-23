// Họ và tên: Lê Hoàng Anh
// Mã sinh viên: 23103100214
// Nội dung thực hiện: Khai báo entity GiaoTra.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class GiaoTra
{
    [Key] public int MaGiaoTra { get; set; }
    public int MaDonGiat { get; set; }
    public DonGiat? DonGiat { get; set; }
    public DateTime NgayGiaoTra { get; set; } = DateTime.Now;
    [StringLength(100)] public string? NguoiGiao { get; set; }
    [StringLength(100)] public string? NguoiNhan { get; set; }
    [StringLength(50)] public string? HinhThucNhan { get; set; }
    [StringLength(300)] public string? GhiChu { get; set; }
}
