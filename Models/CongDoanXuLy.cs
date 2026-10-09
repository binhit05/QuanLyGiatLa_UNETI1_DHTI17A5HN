using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models
{
    public static class TrangThaiCongDoan
    {
        public const string ChuaBatDau = "ChuaBatDau";
        public const string DangXuLy = "DangXuLy";
        public const string HoanThanh = "HoanThanh";
        public const string DaHuy = "DaHuy";
    }
    public class CongDoanXuLy : IValidatableObject
    {
        [Key]
        [Display(Name = "Mã công đoạn")]
        public int MaCongDoan {  get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã đơn giặt không hợp lệ")]
        [Display(Name = "Mã đơn giặt")]
        public int MaDonGiat { get; set; }

        [Required(ErrorMessage = "Tên công đoạn là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên công đoạn tối đa 100 ký tự")]
        [Display(Name = "Tên công đoạn")]
        public string TenCongDoan { get; set; } = string.Empty;

        [Display(Name = "Thời gian bắt đầu")]
        public DateTime? ThoiGianBatDau { get; set; }

        [Display(Name = "Thời gian kết thúc")]
        public DateTime? ThoiGianKetThuc { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Mã nhân viên không hợp lệ")]
        [Display(Name = "Mã nhân viên")]
        public int MaNhanVien { get; set; }

        [Required(ErrorMessage = "Trạng thái công đoạn là bắt buộc")]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = TrangThaiCongDoan.ChuaBatDau;

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(ThoiGianBatDau.HasValue
                && ThoiGianKetThuc.HasValue
                && ThoiGianKetThuc.Value < ThoiGianBatDau.Value)
            {
                yield return new ValidationResult("Thời gian kết thúc không được trước thời gian bắt đầu.", new[] { nameof(ThoiGianKetThuc) });
            }

            if(TrangThai == TrangThaiCongDoan.HoanThanh
                && !ThoiGianKetThuc.HasValue)
            {
                yield return new ValidationResult("Phải có thời gian kết thúc trước khi hoàn thành công đoạn.", new[] { nameof(ThoiGianKetThuc) });
            }
        }
    }
}
