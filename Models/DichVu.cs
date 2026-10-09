using System.ComponentModel.DataAnnotations;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models
{
    public class DichVu
    {
        [Key]
        public int MaDichVu { get; set; }

        [Required(ErrorMessage = "Tên dịch vụ không được để trống")]
        [StringLength(100, ErrorMessage = "Tên dịch vụ không vượt quá 100 ký tự")]
        public string TenDichVu { get; set; } = string.Empty;

        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true; // True: Đang hoạt động, False: Ngưng hoạt động

        // Liên kết với Bảng giá dịch vụ
        public ICollection<BangGiaDichVu>? BangGiaDichVus { get; set; }
    }
}
