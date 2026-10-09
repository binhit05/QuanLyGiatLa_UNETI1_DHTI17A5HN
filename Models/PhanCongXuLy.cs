using System.ComponentModel.DataAnnotations;
using System;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models
{
    public static class TrangThaiPhanCong
    {
        public const string ChuaBatDau = "ChuaBatDau";
        public const string DangXuLy = "DangXuLy";
        public const string HoanThanh = "HoanThanh";
        public const string DaHuy = "DaHuy";
    }
    public class PhanCongXuLy
    {
        [Key]
        [Display(Name = "Mã phân công")]
        public int MaPhanCong { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã đơn giặt không hợp lệ")]
        [Display(Name = "Mã đơn giặt")]
        public int MaDonGiat {  get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã nhân viên không hợp lệ")]
        [Display(Name = "Mã nhân viên")]
        public int MaNhanVien {  get; set; }

        [Display(Name = "Ngày phân công")]
        public DateTime NgayPhanCong { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Nội dung công việc là bắt buộc")]
        [StringLength(300, ErrorMessage = "Nội dung tối đa 300 ký tự")]
        [Display(Name = "Nội dung công việc")]
        public string NoiDungCongViec { get; set; } = string.Empty;

        [Required(ErrorMessage = "Trạng thái phân công là bắt buộc")]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = TrangThaiPhanCong.ChuaBatDau;

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Ghi chú")]
        public string? GhiChu {  get; set; }
    }
}
