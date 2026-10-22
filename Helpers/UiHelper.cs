// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: Định dạng tiền/ngày, mã hiển thị, huy hiệu trạng thái dùng chung cho View (việc nhóm trưởng).
using System.Globalization;
using System.Net;
using Microsoft.AspNetCore.Html;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;

public static class UiHelper
{
    static readonly CultureInfo Vi = new CultureInfo("vi-VN");
    static string H(string? s) => WebUtility.HtmlEncode(s ?? "");

    // ---- định dạng ----
    public static string Tien(decimal v) => v.ToString("N0", Vi) + " đ";
    public static string TienRutGon(decimal v) =>
        v >= 1000000m ? (v / 1000000m).ToString("0.##", Vi) + " tr" : Tien(v);
    public static string Ngay(DateTime d) => d.ToString("dd/MM/yyyy");
    public static string NgayGio(DateTime d) => d.ToString("dd/MM/yyyy, HH:mm");
    public static string So(decimal v) => v.ToString("0.##", Vi);

    // ---- mã hiển thị ----
    public static string MaDon(DonGiat d) => "DG-" + d.NgayTiepNhan.ToString("yyMMdd") + "-" + d.MaDonGiat.ToString("000");
    public static string MaKH(int id) => "KH-" + id.ToString("0000");
    public static string MaDV(int id) => "DV-" + id.ToString("000");
    public static string MaNV(int id) => "NV-" + id.ToString("000");
    public static string MaTK(int id) => "TK-" + id.ToString("000");
    public static string MaDM(string prefix, int id) => prefix + "-" + id.ToString("000");
    public static string MaTT(int donId) => "TT-" + (25000 + donId);
    public static string MaKN(int id) => "KN-" + id.ToString("0000");

    // Lấy số id từ chuỗi người dùng gõ ("DG-240613-082" => 82, "KH-0042" => 42, "42" => 42)
    public static int? ThamSoMa(string s, string prefix)
    {
        s = s.Trim();
        string phan;
        if (s.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) phan = s.Split('-').Last();
        else if (s.Length > 0 && s.All(char.IsDigit)) phan = s;
        else return null;
        return int.TryParse(phan, out var n) ? n : (int?)null;
    }

    // ---- chữ cái avatar ----
    public static string ChuCai(string? ten)
    {
        if (string.IsNullOrWhiteSpace(ten)) return "?";
        var p = ten.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return p[p.Length - 1].Substring(0, 1).ToUpper();
    }

    public static string VietTat(string? ten)
    {
        if (string.IsNullOrWhiteSpace(ten)) return "?";
        var p = ten.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (p.Length == 1) return p[0].Substring(0, 1).ToUpper();
        return (p[0].Substring(0, 1) + p[p.Length - 1].Substring(0, 1)).ToUpper();
    }

    public static string TenCuoi(string? ten)
    {
        if (string.IsNullOrWhiteSpace(ten)) return "";
        var p = ten.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return p[p.Length - 1];
    }

    // ---- huy hiệu trạng thái ----
    public static string BadgeClass(string? t)
    {
        switch (t)
        {
            case "Hoạt động": case "Hoàn thành": case "Đã thanh toán": case "Đã giải quyết": case "Đã giao":
                return "ok";
            case "Đang xử lý": case "Một phần": case "Trung bình": case "Đang thực hiện":
                return "warn";
            case "Chờ giao trả": case "Đã xác nhận": case "Mới tiếp nhận":
                return "info";
            case "Quá hạn": case "Cao": case "Đã khóa": case "Đã hủy":
                return "danger";
            default:
                return "muted";
        }
    }

    public static HtmlString Badge(string? t)
    {
        return new HtmlString("<span class='st st-" + BadgeClass(t) + "'><i></i>" + H(t) + "</span>");
    }

    public static HtmlString Nguoi(string? ten, string? phu = null)
    {
        var s = "<div class='person'><span class='av'>" + H(ChuCai(ten)) + "</span><div><div class='pn'>" + H(ten) + "</div>";
        if (!string.IsNullOrEmpty(phu)) s += "<div class='ps'>" + H(phu) + "</div>";
        s += "</div></div>";
        return new HtmlString(s);
    }

    public static HtmlString TenChuaAvatar(string? ten)
    {
        return new HtmlString("<div class='person'><span class='av'>" + H(ChuCai(ten)) + "</span><div class='pn'>" + H(ten) + "</div></div>");
    }

    // ---- bỏ dấu tiếng Việt (dùng cho email mẫu) ----
    public static string BoDau(string s)
    {
        var n = s.Normalize(System.Text.NormalizationForm.FormD);
        var sb = new System.Text.StringBuilder();
        foreach (var c in n)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) sb.Append(c);
        }
        return sb.ToString().Normalize(System.Text.NormalizationForm.FormC).Replace('đ', 'd').Replace('Đ', 'D');
    }
}
