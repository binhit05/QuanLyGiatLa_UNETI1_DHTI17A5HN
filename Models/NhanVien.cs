// Họ và tên: Nghiêm Xuân Bằng
// Mã sinh viên: .....................
// Nội dung thực hiện: Khai báo entity NhanVien.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class NhanVien
{
    [Key] public int MaNhanVien { get; set; }
    public int? MaTaiKhoan { get; set; }
    public TaiKhoan? TaiKhoan { get; set; }
    [Required(ErrorMessage = "Nhập họ tên"), StringLength(100)] public string HoTen { get; set; } = "";
    [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có 10-11 số"), StringLength(20)] public string? SoDienThoai { get; set; }
    [EmailAddress(ErrorMessage = "Email không đúng định dạng"), StringLength(100)] public string? Email { get; set; }
    [StringLength(50)] public string? ChucVu { get; set; }
    public bool TrangThai { get; set; } = true;
}
