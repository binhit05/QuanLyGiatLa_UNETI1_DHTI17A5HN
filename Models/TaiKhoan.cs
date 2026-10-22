// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: Khai báo entity TaiKhoan.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class TaiKhoan
{
    [Key] public int MaTaiKhoan { get; set; }
    [Required(ErrorMessage = "Nhập tên đăng nhập"), StringLength(50)] public string TenDangNhap { get; set; } = "";
    [Required(ErrorMessage = "Nhập mật khẩu"), StringLength(100)] public string MatKhau { get; set; } = "";
    [Required(ErrorMessage = "Nhập họ tên"), StringLength(100)] public string HoTen { get; set; } = "";
    [EmailAddress(ErrorMessage = "Email không đúng định dạng"), StringLength(100)] public string? Email { get; set; }
    [Required, StringLength(30)] public string VaiTro { get; set; } = VaiTroConst.NhanVienTiepNhan;
    public bool TrangThai { get; set; } = true;
    public DateTime? LanDangNhapCuoi { get; set; }
}

public static class VaiTroConst
{
    public const string Admin = "Admin";
    public const string NhanVienTiepNhan = "NhanVienTiepNhan";
    public const string NhanVienXuLy = "NhanVienXuLy";

    public static string Ten(string? vaiTro) => vaiTro switch
    {
        Admin => "Admin",
        NhanVienTiepNhan => "Tiếp nhận",
        NhanVienXuLy => "Xử lý",
        _ => vaiTro ?? ""
    };
}
