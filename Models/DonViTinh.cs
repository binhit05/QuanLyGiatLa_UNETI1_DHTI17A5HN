// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: Entity DonViTinh (kg, chiếc, bộ, đôi): tên không trùng, chỉ đơn vị đang hoạt động mới dùng cho bảng giá mới.

using System.ComponentModel.DataAnnotations;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class DonViTinh
{
    [Key]
    [Display(Name = "Mã đơn vị tính")]
    public int MaDonViTinh { get; set; }

    [Required(ErrorMessage = "Tên đơn vị tính là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên đơn vị tính tối đa 50 ký tự")]
    [Display(Name = "Tên đơn vị tính")]
    public string TenDonViTinh { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Display(Name = "Đang hoạt động")]
    public bool TrangThai { get; set; } = true;
}