// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: ViewModel của form tài khoản và danh mục (loại đồ, đơn vị tính).
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;

public class TaiKhoanFormVM
{
    public int MaTaiKhoan { get; set; }
    [Required(ErrorMessage = "Nhập tên đăng nhập"), StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3 đến 50 ký tự")] public string TenDangNhap { get; set; } = "";
    [Required(ErrorMessage = "Nhập họ tên"), StringLength(100)] public string HoTen { get; set; } = "";
    [EmailAddress(ErrorMessage = "Email không đúng định dạng"), StringLength(100)] public string? Email { get; set; }
    [Required(ErrorMessage = "Chọn vai trò")] public string VaiTro { get; set; } = "";
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 ký tự")] public string? MatKhau { get; set; }
    public bool TrangThai { get; set; } = true;
}

public class DanhMucForm
{
    public int Id { get; set; }
    public string Tab { get; set; } = "loaido";
    [Required(ErrorMessage = "Nhập tên danh mục"), StringLength(100)] public string Ten { get; set; } = "";
    [StringLength(300)] public string? MoTa { get; set; }
    public bool TrangThai { get; set; } = true;
}

public class DanhMucRow
{
    public int Id { get; set; }
    public string Ma { get; set; } = "";
    public string Ten { get; set; } = "";
    public string? MoTa { get; set; }
    public int DangSuDung { get; set; }
    public bool TrangThai { get; set; }
}
