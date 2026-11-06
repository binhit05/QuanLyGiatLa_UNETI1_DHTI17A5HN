// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: Đọc Session: mã tài khoản, họ tên, vai trò.
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;

public static class SessionHelper
{
    public const string KeyMa = "MaTaiKhoan", KeyTen = "HoTen", KeyVaiTro = "VaiTro";
    public static int? LayMaTaiKhoan(this ISession s) => s.GetInt32(KeyMa);
    public static string? LayVaiTro(this ISession s) => s.GetString(KeyVaiTro);
    public static string? LayHoTen(this ISession s) => s.GetString(KeyTen);
}
