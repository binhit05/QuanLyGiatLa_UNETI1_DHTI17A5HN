// Họ và tên: Lê Duy Khánh
// Mã sinh viên: 23103100216
// Nội dung thực hiện: Entity KhieuNai (khiếu nại đơn giặt)
// kiểm tra ngày hoàn tất và hằng số trạng thái/mức độ.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models
{
    public class KhieuNai : IValidatableObject
    {
        [Key]
        [Display(Name = "Mã khiếu nại")]
        public int MaKhieuNai { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn đơn giặt")]
        [Display(Name = "Mã đơn giặt")]
        public int MaDonGiat { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Vui lòng nhập nội dung khiếu nại")]
        [StringLength(1000, ErrorMessage = "Nội dung tối đa 1000 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn mức độ")]
        [StringLength(50)]
        [Display(Name = "Mức độ")]
        public string MucDo { get; set; } = MucDoKhieuNai.TrungBinh;

        [Required]
        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = TrangThaiKhieuNai.MoiTiepNhan;

        [StringLength(1000, ErrorMessage = "Hướng xử lý tối đa 1000 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Hướng xử lý")]
        public string? HuongXuLy { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày hoàn tất")]
        public DateTime? NgayHoanTat { get; set; }
        //Validation tùy chỉnh: ngày hoàn tất (nếu có) không được trước ngày tạo
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NgayHoanTat.HasValue && NgayHoanTat.Value < NgayTao)
            {
                yield return new ValidationResult(
                    "Ngày hoàn tất không được trước ngày tạo",
                    new[] { nameof(NgayHoanTat) });
            }
        }
    }
    public static class TrangThaiKhieuNai
    {
        public const string MoiTiepNhan = "Mới tiếp nhận";
        public const string DangXuLy = "Đang xử lý";
        public const string DaGiaiQuyet = "Đã giải quyết";
        public const string TuChoi = "Từ chối";
    }

    public static class MucDoKhieuNai
    {
        public const string Thap = "Thấp";
        public const string TrungBinh = "Trung bình";
        public const string Cao = "Cao";

        public static readonly string[] DanhSach = { Thap, TrungBinh, Cao };
    }
}
