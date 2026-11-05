// Họ và tên: Lê Duy Khánh
// Mã sinh viên: 23103100216
// Nội dung thực hiện: Khai báo entity ThanhToan.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class ThanhToan
{
    [Key] public int MaThanhToan { get; set; }
    public int MaDonGiat { get; set; }
    public DonGiat? DonGiat { get; set; }
    public DateTime NgayThanhToan { get; set; } = DateTime.Now;
    [Range(1, 1000000000, ErrorMessage = "Số tiền phải lớn hơn 0")] public decimal SoTien { get; set; }
    [StringLength(50)] public string PhuongThuc { get; set; } = "Tiền mặt";
    [StringLength(100)] public string? NguoiThucHien { get; set; }
    [StringLength(300)] public string? GhiChu { get; set; }
}
