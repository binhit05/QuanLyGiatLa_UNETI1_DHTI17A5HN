// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: Các ViewModel dùng cho biểu mẫu và trang thống kê.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;

public class DangNhapViewModel
{
    [Required(ErrorMessage = "Nhập tên đăng nhập")] public string TenDangNhap { get; set; } = "";
    [Required(ErrorMessage = "Nhập mật khẩu")] public string MatKhau { get; set; } = "";
}
