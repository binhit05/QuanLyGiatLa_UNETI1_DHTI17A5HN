// Họ và tên: Lê Duy Khánh
// Mã sinh viên: 23103100216
// Nội dung thực hiện: ViewModel thanh toán, Dashboard và thống kê báo cáo.
using System.ComponentModel.DataAnnotations;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;

public class ThanhToanVM
{
    [Range(1, int.MaxValue, ErrorMessage = "Chọn đơn giặt")] public int MaDonGiat { get; set; }
    [Range(1, 1000000000, ErrorMessage = "Số tiền phải lớn hơn 0")] public decimal SoTien { get; set; }
    [Required(ErrorMessage = "Chọn phương thức")] public string PhuongThuc { get; set; } = "Tiền mặt";
    [StringLength(300)] public string? GhiChu { get; set; }
}

public class DashboardVM
{
    public int DonHomNay, DonHomQua, DangXuLy, SapDenHan, ChoGiaoTra, QuaHan, KhieuNaiDangXuLy, KhieuNaiCao, SoDonConThu;
    public decimal DoanhThuHomNay, DoanhThuHomQua, DoanhThuThangNay, DoanhThuThangTruoc, ConPhaiThu;
    public List<string> Nhan = new();
    public List<decimal> Gia7 = new();
    public List<decimal> Gia30 = new();
    public List<string> Nhan30 = new();
    public List<DichVuTyLe> DichVuPhoBien = new();
    public List<Models.DonGiat> DonGanDay = new();
}

public class DichVuTyLe { public string Ten { get; set; } = ""; public int TyLe { get; set; } }

public class DongThongKe
{
    public string Ten { get; set; } = "";
    public int SoLuong { get; set; }
    public decimal TongTien { get; set; }
    public decimal TyLe { get; set; }
}
// Khối lượng/số lượng theo loại đồ (nhóm theo loại đồ + đơn vị tính vì kg, chiếc, bộ không cộng chung được)
public class DongLoaiDo
{
    public string TenLoaiDo { get; set; } = "";
    public string TenDonViTinh { get; set; } = "";
    public int SoDong { get; set; }
    public decimal TongSoLuongKhoiLuong { get; set; }
}

// Số đơn quá hẹn theo tháng (tháng của NgayHenTra)
public class DongThang
{
    public string Thang { get; set; } = "";
    public int TongDon { get; set; }
    public int SoQuaHen { get; set; }
    public decimal TyLe { get; set; }
}

// Số công việc hoàn thành theo nhân viên
public class DongNhanVien
{
    public int MaNhanVien { get; set; }
    public string HoTen { get; set; } = "";
    public int SoPhanCongHoanThanh { get; set; }
    public int SoCongDoanHoanThanh { get; set; }
}

// Tỷ lệ khiếu nại theo dịch vụ
public class DongKhieuNai
{
    public string TenDichVu { get; set; } = "";
    public int SoDon { get; set; }
    public int SoKhieuNai { get; set; }
    public decimal TyLe { get; set; }
}
