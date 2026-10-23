// Họ và tên: Lê Hoàng Anh
// Mã sinh viên: 23103100214
// Nội dung thực hiện: ViewModel phân công xử lý, dòng theo dõi tiến độ và giao trả.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;

public class PhanCongVM
{
    [Range(1, int.MaxValue, ErrorMessage = "Chọn đơn giặt")] public int MaDonGiat { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Chọn nhân viên xử lý")] public int MaNhanVien { get; set; }
    [StringLength(300)] public string? NoiDungCongViec { get; set; }
}

public class PhanCongRow
{
    public Models.DonGiat Don { get; set; } = new Models.DonGiat();
    public string NhanVien { get; set; } = "";
    public int TienDo { get; set; }
    public string CongDoanHienTai { get; set; } = "";
    public bool Nguy { get; set; }
}

public class GiaoTraVM
{
    [Range(1, int.MaxValue, ErrorMessage = "Chọn đơn giặt")] public int MaDonGiat { get; set; }
    public DateTime NgayGiaoTra { get; set; } = DateTime.Now;
    [StringLength(100)] public string? NguoiGiao { get; set; }
    [Required(ErrorMessage = "Nhập tên người nhận"), StringLength(100)] public string NguoiNhan { get; set; } = "";
    [StringLength(50)] public string HinhThucNhan { get; set; } = "Nhận tại cửa hàng";
    [StringLength(300)] public string? GhiChu { get; set; }
}
