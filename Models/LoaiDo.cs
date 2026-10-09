// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: Entity LoaiDo (loại đồ giặt): tên không trùng, ngừng hoạt động thì không dùng cho chi tiết mới.

using System.ComponentModel.DataAnnotations;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class LoaiDo
{
    [Key]
    [Display(Name = "Mã loại đồ")]
    public int MaLoaiDo { get; set; }

    [Required(ErrorMessage = "Tên loại đồ là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên loại đồ tối đa 100 ký tự")]
    [Display(Name = "Tên loại đồ")]
    public string TenLoaiDo { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
    [Display(Name = "Mô tả")]
    public string? MoTa { get; set; }

    [Display(Name = "Đang hoạt động")]
    public bool TrangThai { get; set; } = true;
}