// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: Kiểm tra đăng nhập và phân quyền theo vai trò tại Controller.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Filters;

// Kiểm tra đăng nhập và vai trò ở phía Controller (không chỉ ẩn menu)
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class PhanQuyenAttribute : ActionFilterAttribute
{
    private readonly string[] _vaiTros;
    public PhanQuyenAttribute(params string[] vaiTros) => _vaiTros = vaiTros;

    public override void OnActionExecuting(ActionExecutingContext ctx)
    {
        var session = ctx.HttpContext.Session;
        if (session.LayMaTaiKhoan() == null)
        {
            ctx.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", null);
            return;
        }
        if (_vaiTros.Length > 0 && !_vaiTros.Contains(session.LayVaiTro()))
            ctx.Result = new RedirectToActionResult("TuChoiTruyCap", "TaiKhoan", null);
    }
}
