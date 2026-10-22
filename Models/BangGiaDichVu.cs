// Họ và tên: Nghiêm Xuân Bằng
// Mã sinh viên: .....................
// Nội dung thực hiện: Khai báo entity BangGiaDichVu.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class BangGiaDichVu
{
    [Key] public int MaBangGia { get; set; }
    public int MaDichVu { get; set; }
    public DichVu? DichVu { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Chọn loại đồ")] public int MaLoaiDo { get; set; }
    public LoaiDo? LoaiDo { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Chọn đơn vị tính")] public int MaDonViTinh { get; set; }
    public DonViTinh? DonViTinh { get; set; }
    [Range(1, 100000000, ErrorMessage = "Đơn giá phải lớn hơn 0")] public decimal DonGia { get; set; }
    [DataType(DataType.Date)] public DateTime NgayApDung { get; set; } = DateTime.Today;
    [DataType(DataType.Date)] public DateTime? NgayKetThuc { get; set; }
    public bool TrangThai { get; set; } = true;
}
