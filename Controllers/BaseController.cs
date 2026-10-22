// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: Lớp cơ sở cho các Controller (đọc session, thông báo, ghi lịch sử xử lý).
using Microsoft.AspNetCore.Mvc;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers;

public abstract class BaseController : Controller
{
    protected readonly AppDbContext _db;
    protected const int CoTrang = 8;
    protected BaseController(AppDbContext db) { _db = db; }

    protected string TenNguoiDung => HttpContext.Session.LayHoTen() ?? "";
    protected int? MaNguoiDung => HttpContext.Session.LayMaTaiKhoan();
    protected string VaiTroHienTai => HttpContext.Session.LayVaiTro() ?? "";
    protected bool LaAdmin => VaiTroHienTai == VaiTroConst.Admin;

    protected void ThongBao(string noiDung) => TempData["Msg"] = noiDung;
    protected void BaoLoi(string noiDung) => TempData["Loi"] = noiDung;

    // Ghi lịch sử xử lý của đơn (chưa SaveChanges, gọi SaveChanges ở nơi dùng)
    protected void GhiLichSu(int maDon, string noiDung)
    {
        _db.LichSuXuLys.Add(new LichSuXuLy
        {
            MaDonGiat = maDon, NoiDung = noiDung, ThoiGian = DateTime.Now, NguoiThucHien = TenNguoiDung
        });
    }
}
