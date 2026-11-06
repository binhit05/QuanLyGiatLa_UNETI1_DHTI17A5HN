// Họ và tên: Lê Hoàng Anh
// Mã sinh viên: 23103100214
// Nội dung thực hiện: Quy trình chuyển trạng thái đơn giặt và danh sách 6 công đoạn xử lý.
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;

// Quy trình trạng thái đơn giặt và các công đoạn xử lý
public static class QuyTrinh
{
    public const string ChuaBatDau = "Chưa bắt đầu";
    public const string DangThucHien = "Đang thực hiện";
    public const string HoanThanh = "Hoàn thành";

    public static readonly string[] CongDoans =
    {
        "Phân loại", "Xử lý vết bẩn", "Giặt / hấp", "Sấy khô", "Là / ủi", "Kiểm tra & đóng gói"
    };

    // Các trạng thái được phép chuyển tiếp từ trạng thái hiện tại
    public static string[] TrangThaiTiepTheo(string hienTai)
    {
        switch (hienTai)
        {
            case TrangThaiDon.MoiTiepNhan: return new[] { TrangThaiDon.DaXacNhan, TrangThaiDon.DaHuy };
            case TrangThaiDon.DaXacNhan: return new[] { TrangThaiDon.DangXuLy, TrangThaiDon.DaHuy };
            case TrangThaiDon.DangXuLy: return new[] { TrangThaiDon.ChoGiaoTra, TrangThaiDon.DaHuy };
            case TrangThaiDon.ChoGiaoTra: return new[] { TrangThaiDon.DaHuy };
            default: return Array.Empty<string>();
        }
    }
}
