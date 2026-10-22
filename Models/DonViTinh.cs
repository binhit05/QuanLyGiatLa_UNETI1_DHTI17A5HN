// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: Khai báo entity DonViTinh.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class DonViTinh
{
    [Key] public int MaDonViTinh { get; set; }
    [Required(ErrorMessage = "Nhập tên đơn vị tính"), StringLength(50)] public string TenDonViTinh { get; set; } = "";
    [StringLength(300)] public string? MoTa { get; set; }
    public bool TrangThai { get; set; } = true;
}
