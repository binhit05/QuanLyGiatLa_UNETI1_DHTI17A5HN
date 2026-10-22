using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class KhachHang
{
    [Key] public int MaKhachHang { get; set; }
    [Required(ErrorMessage = "Nhập họ tên khách hàng"), StringLength(100)] public string HoTen { get; set; } = "";
    [Required(ErrorMessage = "Nhập số điện thoại"), RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có 10-11 số"), StringLength(20)] public string SoDienThoai { get; set; } = "";
    [EmailAddress(ErrorMessage = "Email không đúng định dạng"), StringLength(100)] public string? Email { get; set; }
    [StringLength(200)] public string? DiaChi { get; set; }
    [StringLength(300)] public string? GhiChu { get; set; }
    public bool TrangThai { get; set; } = true;
    public List<DonGiat> DonGiats { get; set; } = new();
}
