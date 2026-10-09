using System.ComponentModel.DataAnnotations;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models
{
    public class NhanVien
    {
        [Key]
        public int MaNhanVien { get; set; }

        [Required(ErrorMessage = "Họ tên nhân viên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        public string? ChucVu { get; set; }

        public bool TrangThai { get; set; } = true;
    }
}
