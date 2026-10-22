// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: Khai báo entity LoaiDo.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class LoaiDo
{
    [Key] public int MaLoaiDo { get; set; }
    [Required(ErrorMessage = "Nhập tên loại đồ"), StringLength(100)] public string TenLoaiDo { get; set; } = "";
    [StringLength(300)] public string? MoTa { get; set; }
    public bool TrangThai { get; set; } = true;
}
