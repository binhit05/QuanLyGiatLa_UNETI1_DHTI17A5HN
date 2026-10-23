// Họ và tên: Lê Duy Khánh
// Mã sinh viên: 23103100216
// Nội dung thực hiện: Khai báo entity LichSuXuLy.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

// Lịch sử xử lý của đơn: ghi tự động, không cho sửa/xóa
public class LichSuXuLy
{
    [Key] public int MaLichSu { get; set; }
    public int MaDonGiat { get; set; }
    public DonGiat? DonGiat { get; set; }
    [Required, StringLength(500)] public string NoiDung { get; set; } = "";
    public DateTime ThoiGian { get; set; } = DateTime.Now;
    [StringLength(100)] public string? NguoiThucHien { get; set; }
}
