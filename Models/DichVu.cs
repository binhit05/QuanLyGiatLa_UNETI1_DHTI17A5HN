// Họ và tên: Nghiêm Xuân Bằng
// Mã sinh viên: .....................
// Nội dung thực hiện: Khai báo entity DichVu.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class DichVu
{
    [Key] public int MaDichVu { get; set; }
    [Required(ErrorMessage = "Nhập tên dịch vụ"), StringLength(100)] public string TenDichVu { get; set; } = "";
    [StringLength(500)] public string? MoTa { get; set; }
    [Range(1, 720, ErrorMessage = "Thời gian xử lý (giờ) từ 1 đến 720")] public int ThoiGianXuLyDuKien { get; set; } = 24;
    public bool TrangThai { get; set; } = true;
}
