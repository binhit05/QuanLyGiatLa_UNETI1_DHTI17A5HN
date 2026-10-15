// Họ và tên: Lê Duy Khánh
// Mã sinh viên: 23103100216
// Nội dung thực hiện: form ghi nhận thanh toán, kèm tổng tiền đơn, tổng đã thanh toán, số tiền còn lại và trạng thái thanh toán
using System.ComponentModel.DataAnnotations;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels
{
    public class ThanhToanCreateViewModel
    {
        //Dữ liệu người dùng nhập
        [Range(1,int.MaxValue, ErrorMessage = "Vui lòng chọn đơn giặt")]
        [Display(Name ="Đơn giặt")]
        public int MaDonGiat {  get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày thanh toán")]
        public DateTime NgayThanhToan { get; set; } = DateTime.Now;

        [Range(typeof(decimal), "0.01", "999999999999.99",
            ErrorMessage = "Số tiền thanh toán phải lớn hơn 0")]
        [Display(Name = "Số tiền")]
        public decimal SoTien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [StringLength(50)]
        [Display(Name = "Phương thức")]
        public string PhuongThuc { get; set; } = PhuongThucThanhToan.TienMat;

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        //Dữ liệu chỉ hiển thị (Controller tự tính lại,không tin dữ liệu từ form)
        public string? TenKhachHang {  get; set; }
        public decimal TongTienDon {  get; set; }
        public decimal TongDaThanhToan { get; set; }
        public decimal SoTienConLai {  get; set; }
        public string? TrangThaiThanhToan { get; set; }

        // Danh sách các đơn còn nợ để đổ vào select
        public List<DonCanThanhToanItem> DanhSachDon { get; set; } = new();

    }
    public class DonCanThanhToanItem
    {
        public int MaDonGiat { get; set; }
        public string? TenKhachHang { get; set; }
        public decimal TongTien { get; set; }
        public decimal DaThanhToan { get; set; }
        public decimal ConLai => TongTien - DaThanhToan;
    }
    // ViewModel trang chi tiết một khoản thanh toán + tình hình thanh toán của cả đơn
    public class ThanhToanDetailsViewModel
    {
        public ThanhToan ThanhToan { get; set; } = null!;
        public string? TenKhachHang { get; set; }
        public decimal TongTienDon { get; set; }
        public decimal TongDaThanhToan { get; set; }
        public decimal SoTienConLai { get; set; }
        public string TrangThaiThanhToan { get; set; } = string.Empty;
        public List<ThanhToan> LichSuThanhToan { get; set; } = new();
    }
}
