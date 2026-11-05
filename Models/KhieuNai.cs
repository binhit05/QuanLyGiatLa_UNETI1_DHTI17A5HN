// Họ và tên: Lê Duy Khánh
// Mã sinh viên: 23103100216
// Nội dung thực hiện: Khai báo entity KhieuNai.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class KhieuNai
{
    [Key] public int MaKhieuNai { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Chọn đơn giặt")] public int MaDonGiat { get; set; }
    public DonGiat? DonGiat { get; set; }
    public DateTime NgayTao { get; set; } = DateTime.Now;
    [Required(ErrorMessage = "Nhập nội dung khiếu nại"), StringLength(500)] public string NoiDung { get; set; } = "";
    [StringLength(20)] public string MucDo { get; set; } = "Thấp";
    [StringLength(30)] public string TrangThai { get; set; } = "Mới tiếp nhận";
    [StringLength(100)] public string? NguoiPhuTrach { get; set; }
    [StringLength(500)] public string? HuongXuLy { get; set; }
    public DateTime? NgayHoanTat { get; set; }
}
