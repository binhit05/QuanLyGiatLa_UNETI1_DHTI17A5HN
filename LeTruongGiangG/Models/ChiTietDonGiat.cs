using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

public class ChiTietDonGiat
{
    [Key] public int MaChiTiet { get; set; }
    public int MaDonGiat { get; set; }
    public DonGiat? DonGiat { get; set; }
    public int MaDichVu { get; set; }
    public DichVu? DichVu { get; set; }
    public int MaLoaiDo { get; set; }
    public LoaiDo? LoaiDo { get; set; }
    public int MaDonViTinh { get; set; }
    public DonViTinh? DonViTinh { get; set; }
    [Range(0.01, 100000, ErrorMessage = "Số lượng/khối lượng phải lớn hơn 0")] public decimal SoLuongKhoiLuong { get; set; }
    public decimal DonGiaApDung { get; set; }
    public decimal ThanhTien { get; set; }
    [StringLength(300)] public string? TinhTrangLucNhan { get; set; }
    [StringLength(300)] public string? YeuCauDacBiet { get; set; }
}
