// Họ và tên: Lê Duy Khánh
// Mã sinh viên: 23103100216
// Nội dung thực hiện: Entity ThanhToan (thanh toán đơn giặt)
// và hằng số phương thức thanh toán.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models
{
    public class ThanhToan
    {
        [Key]
        [Display(Name = "Mã thanh toán")]
        public int MaThanhToan { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn đơn giặt")]
        [Display(Name = "Mã đơn giặt")]
        public int MaDonGiat { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày thanh toán")]
        public DateTime NgayThanhToan { get; set; } = DateTime.Now;

        [Range(typeof(decimal), "0.01", "999999999999.99",
            ErrorMessage = "Số tiền thanh toán phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Số tiền")]
        public decimal SoTien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [StringLength(50, ErrorMessage = "Phương thức tối đa 50 ký tự")]
        [Display(Name = "Phương thức")]
        public string PhuongThuc { get; set; } = null!;

        [Required(ErrorMessage = "Thiếu người thực hiện")]
        [StringLength(100, ErrorMessage = "Tên người thực hiện tối đa 100 ký tự")]
        [Display(Name = "Người thực hiện")]
        public string NguoiThucHien { get; set; } = null!;

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
    }
    // Hằng số để tránh gõ chuỗi tay 
    public static class PhuongThucThanhToan
    {
        public const string TienMat = "Tiền mặt";
        public const string ChuyenKhoan = "Chuyển khoản";
        public const string The = "Thẻ";

        public static readonly string[] DanhSach = { TienMat, ChuyenKhoan, The };
    }
}
