// Họ và tên: Lê Trường Giang
// Mã sinh viên: .....................
// Nội dung thực hiện: ViewModel tạo đơn giặt (dòng chi tiết, thông tin đơn) và chỉnh sửa đơn.
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;

public class DongCT
{
    public int MaDichVu { get; set; }
    public int MaLoaiDo { get; set; }
    public decimal SoLuong { get; set; }
    public string? TinhTrang { get; set; }
    // Danh sách ảnh tình trạng đồ lúc tiếp nhận
    public List<IFormFile> AnhTinhTrangs { get; set; } = new();
}

public class TaoDonVM
{
    [Required(ErrorMessage = "Chọn khách hàng")] public int? MaKhachHang { get; set; }
    public DateTime NgayTiepNhan { get; set; } = DateTime.Now;
    public DateTime NgayHenTra { get; set; } = DateTime.Now.AddDays(1);
    public int? MaNguoiTiepNhan { get; set; }
    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")] public string? GhiChu { get; set; }
    [Range(0, 100000000, ErrorMessage = "Phụ phí không hợp lệ")] public decimal PhuPhi { get; set; }
    public List<DongCT> ChiTiets { get; set; } = new();
}

public class SuaDonVM
{
    public int MaDonGiat { get; set; }
    public DateTime NgayHenTra { get; set; }
    public int? MaNguoiTiepNhan { get; set; }
    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")] public string? GhiChu { get; set; }
    [Range(0, 100000000, ErrorMessage = "Phụ phí không hợp lệ")] public decimal PhuPhi { get; set; }
}
