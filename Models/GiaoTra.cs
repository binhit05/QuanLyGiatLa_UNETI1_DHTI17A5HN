using System.ComponentModel.DataAnnotations;
using System;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models
{
    public class GiaoTra
    {
        [Key]
        [Display(Name = "Mã giao trả")]
        public int MaGiaoTra { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã đơn giặt không hợp lệ")]
        [Display(Name = "Mã đơn giặt")]
        public int MaDonGiat { get; set; }

        [Display(Name = "Ngày giao trả")]
        public DateTime NgayGiaoTra { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Người giao là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên người giao tối đa 100 ký tự")]
        [Display(Name = "Người giao")]
        public string NguoiGiao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Người nhận là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên người nhận tối đa 100 ký tự")]
        [Display(Name = "Người nhận")]
        public string NguoiNhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phương thức giao trả là bắt buộc")]
        [StringLength(50, ErrorMessage = "Phương thức tối đa 50 ký tự")]
        [Display(Name = "Phương thức giao trả")]
        public string PhuongThucGiaoTra { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
    }
}
