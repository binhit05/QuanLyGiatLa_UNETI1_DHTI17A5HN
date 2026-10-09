// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: Entity TaiKhoan và hằng số vai trò dùng cho đăng nhập, Session, phân quyền.

using System.ComponentModel.DataAnnotations;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public static class VaiTroTaiKhoan
{
    public const string Admin = "Admin";
    public const string TiepNhan = "TiepNhan";
    public const string XuLy = "XuLy";
}

public class TaiKhoan
{
    [Key]
    [Display(Name = "Mã tài khoản")]
    public int MaTaiKhoan { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên đăng nhập tối đa 50 ký tự")]
    [Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(256, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 đến 256 ký tự")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    [Display(Name = "Họ và tên")]
    public string HoTen { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Vai trò là bắt buộc")]
    [StringLength(30)]
    [Display(Name = "Vai trò")]
    public string VaiTro { get; set; } = VaiTroTaiKhoan.TiepNhan;

    // true = đang hoạt động, false = bị khóa
    [Display(Name = "Đang hoạt động")]
    public bool TrangThai { get; set; } = true;
}