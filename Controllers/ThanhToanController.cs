// Họ và tên: Lê Duy Khánh
// Mã sinh viên: 23103100216
// Nội dung thực hiện: ThanhToanController - danh sách (Search/Filter/Sort/Pagination),
// ghi nhận thanh toán, kiểm tra không thanh toán vượt số tiền còn lại, xem chi tiết.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;
using System.Globalization;

public class ThanhToanController : Controller
{
    private readonly AppDbContext _context;

    private const int KichThuocTrang = 10;
    private static readonly CultureInfo ViVN = new("vi-VN");

    // ===== CẦN ĐỐI CHIẾU VỚI MODULE 1 (tên khóa Session và giá trị VaiTro) =====
    private const string SessionHoTen = "HoTen";
    private const string SessionVaiTro = "VaiTro";
    private static readonly string[] VaiTroDuocPhep = { "Admin", "Nhân viên tiếp nhận" };

    // ===== CẦN ĐỐI CHIẾU VỚI MODULE 3 (giá trị TrangThai của DonGiat) =====
    private const string TrangThaiDonDaHuy = "Đã hủy";

    public ThanhToanController(AppDbContext context)
    {
        _context = context;
    }
    // Kiểm tra đăng nhập + phân quyền cho TOÀN BỘ Action của Controller (không chỉ ẩn menu)
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var vaiTro = HttpContext.Session.GetString(SessionVaiTro);

        if (string.IsNullOrEmpty(vaiTro))
            context.Result = RedirectToAction("DangNhap", "TaiKhoan");
        else if (!VaiTroDuocPhep.Contains(vaiTro))
            context.Result = StatusCode(StatusCodes.Status403Forbidden);

        base.OnActionExecuting(context);
    }

    // GET: THANHTOANS
    public async Task<IActionResult> Index(string? tuKhoa, string? phuongThuc,
            DateTime? tuNgay, DateTime? denNgay, string? sapXep, int trang = 1)
    {
        var query = _context.ThanhToans
            .AsNoTracking()
            .Include(t => t.DonGiat).ThenInclude(d => d!.KhachHang)
            .AsQueryable();

        // 1. Tìm kiếm: mã đơn, tên/SĐT khách hàng, người thực hiện
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            tuKhoa = tuKhoa.Trim();
            bool laSo = int.TryParse(tuKhoa, out int maDon);
            query = query.Where(t =>
                (laSo && t.MaDonGiat == maDon)
                || t.NguoiThucHien.Contains(tuKhoa)
                || t.DonGiat!.KhachHang!.HoTen.Contains(tuKhoa)
                || t.DonGiat!.KhachHang!.SoDienThoai.Contains(tuKhoa));
        }

        // 2. Lọc: phương thức, khoảng ngày (đến ngày tính trọn ngày)
        if (!string.IsNullOrWhiteSpace(phuongThuc))
            query = query.Where(t => t.PhuongThuc == phuongThuc);

        if (tuNgay.HasValue)
            query = query.Where(t => t.NgayThanhToan >= tuNgay.Value.Date);

        if (denNgay.HasValue)
            query = query.Where(t => t.NgayThanhToan < denNgay.Value.Date.AddDays(1));

        // Tổng tiền của toàn bộ kết quả lọc (tính trên DB, trước khi phân trang)
        decimal tongSoTienLoc = await query.SumAsync(t => (decimal?)t.SoTien) ?? 0m;

        // 3. Sắp xếp
        query = sapXep switch
        {
            "ngay_tang" => query.OrderBy(t => t.NgayThanhToan).ThenBy(t => t.MaThanhToan),
            "tien_giam" => query.OrderByDescending(t => t.SoTien).ThenByDescending(t => t.MaThanhToan),
            "tien_tang" => query.OrderBy(t => t.SoTien).ThenBy(t => t.MaThanhToan),
            "don" => query.OrderBy(t => t.MaDonGiat).ThenByDescending(t => t.NgayThanhToan),
            _ => query.OrderByDescending(t => t.NgayThanhToan).ThenByDescending(t => t.MaThanhToan)
        };

        // 4. Phân trang bằng Skip/Take trên database
        int tongSoBanGhi = await query.CountAsync();
        int tongSoTrang = (int)Math.Ceiling(tongSoBanGhi / (double)KichThuocTrang);

        if (trang > tongSoTrang) trang = tongSoTrang;
        if (trang < 1) trang = 1;

        var danhSach = await query
            .Skip((trang - 1) * KichThuocTrang)
            .Take(KichThuocTrang)
            .ToListAsync();

        var vm = new ThanhToanListViewModel
        {
            DanhSach = danhSach,
            TuKhoa = tuKhoa,
            PhuongThuc = phuongThuc,
            TuNgay = tuNgay,
            DenNgay = denNgay,
            SapXep = sapXep,
            TrangHienTai = trang,
            KichThuocTrang = KichThuocTrang,
            TongSoBanGhi = tongSoBanGhi,
            TongSoTrang = tongSoTrang,
            TongSoTienLoc = tongSoTienLoc
        };

        return View(vm);
    }
    // ================= CHI TIẾT MỘT KHOẢN THANH TOÁN ================

    // GET: THANHTOANS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var thanhToan = await _context.ThanhToans
            .AsNoTracking()
            .Include(t => t.DonGiat).ThenInclude(d => d!.KhachHang)
            .FirstOrDefaultAsync(t => t.MaThanhToan == id);

        if (thanhToan == null) return NotFound();

        // Toàn bộ các lần thanh toán của cùng đơn (đối chiếu)
        var lichSu = await _context.ThanhToans
            .AsNoTracking()
            .Where(t => t.MaDonGiat == thanhToan.MaDonGiat)
            .OrderBy(t => t.NgayThanhToan).ThenBy(t => t.MaThanhToan)
            .ToListAsync();

        decimal tongTienDon = thanhToan.DonGiat?.TongTienDuKien ?? 0m;
        decimal tongDaThanhToan = lichSu.Sum(t => t.SoTien);

        var vm = new ThanhToanDetailsViewModel
        {
            ThanhToan = thanhToan,
            TenKhachHang = thanhToan.DonGiat?.KhachHang?.HoTen,
            TongTienDon = tongTienDon,
            TongDaThanhToan = tongDaThanhToan,
            SoTienConLai = tongTienDon - tongDaThanhToan,
            TrangThaiThanhToan = TrangThaiThanhToan.Tinh(tongTienDon, tongDaThanhToan),
            LichSuThanhToan = lichSu
        };

        return View(vm);
    }
    // ================= GHI NHẬN THANH TOÁN =================

    // GET: THANHTOANS/Create
    [HttpGet]
    public async Task<IActionResult> Create(int? maDonGiat)
    {
        var vm = new ThanhToanCreateViewModel();

        if (maDonGiat.HasValue)
            vm.MaDonGiat = maDonGiat.Value;

        await NapDuLieuHienThi(vm);

        // Gợi ý sẵn số tiền còn lại khi đã biết đơn
        if (vm.MaDonGiat > 0 && vm.SoTienConLai > 0)
            vm.SoTien = vm.SoTienConLai;

        return View(vm);
    }

    // POST: THANHTOANS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ThanhToanCreateViewModel vm)
    {
        // Bước 1: validation bằng Data Annotation (đã khai báo trong ViewModel)
        if (ModelState.IsValid)
        {
            // Bước 2: validation nghiệp vụ (Data Annotation không biểu diễn được)
            var don = await _context.DonGiats
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.MaDonGiat == vm.MaDonGiat);

            if (don == null)
            {
                ModelState.AddModelError(nameof(vm.MaDonGiat), "Đơn giặt không tồn tại");
            }
            else if (don.TrangThai == TrangThaiDonDaHuy)
            {
                ModelState.AddModelError(nameof(vm.MaDonGiat), "Không thể thanh toán cho đơn đã hủy");
            }
            else
            {
                decimal tongDaThanhToan = await _context.ThanhToans
                    .Where(t => t.MaDonGiat == vm.MaDonGiat)
                    .SumAsync(t => (decimal?)t.SoTien) ?? 0m;

                decimal soTienConLai = don.TongTienDuKien - tongDaThanhToan;

                if (soTienConLai <= 0)
                    ModelState.AddModelError(nameof(vm.MaDonGiat), "Đơn này đã được thanh toán đủ");
                else if (vm.SoTien > soTienConLai)
                    ModelState.AddModelError(nameof(vm.SoTien),
                        $"Số tiền vượt quá số còn lại ({soTienConLai.ToString("N0", ViVN)} ₫)");

                if (vm.NgayThanhToan > DateTime.Now.AddMinutes(5))
                    ModelState.AddModelError(nameof(vm.NgayThanhToan), "Ngày thanh toán không được ở tương lai");

                if (vm.NgayThanhToan.Date < don.NgayTiepNhan.Date)
                    ModelState.AddModelError(nameof(vm.NgayThanhToan), "Ngày thanh toán không được trước ngày tiếp nhận đơn");
            }

            if (!PhuongThucThanhToan.DanhSach.Contains(vm.PhuongThuc))
                ModelState.AddModelError(nameof(vm.PhuongThuc), "Phương thức thanh toán không hợp lệ");
        }

        if (!ModelState.IsValid)
        {
            await NapDuLieuHienThi(vm);
            return View(vm);
        }

        // Bước 3: lưu. NguoiThucHien lấy từ Session, không cho người dùng tự nhập
        var thanhToan = new ThanhToan
        {
            MaDonGiat = vm.MaDonGiat,
            NgayThanhToan = vm.NgayThanhToan,
            SoTien = vm.SoTien,
            PhuongThuc = vm.PhuongThuc,
            NguoiThucHien = HttpContext.Session.GetString(SessionHoTen) ?? "Không xác định",
            GhiChu = vm.GhiChu
        };

        _context.ThanhToans.Add(thanhToan);
        await _context.SaveChangesAsync();

        TempData["ThongBao"] =
            $"Đã ghi nhận thanh toán {vm.SoTien.ToString("N0", ViVN)} ₫ cho đơn #{vm.MaDonGiat}.";

        return RedirectToAction(nameof(Details), new { id = thanhToan.MaThanhToan });
    }
    // Không có Edit/Delete: thanh toán là dữ liệu đối chiếu, không được sửa/xóa tùy ý

    // ================= HÀM HỖ TRỢ =================

    // Nạp danh sách đơn còn nợ cho select + tính tổng tiền/đã trả/còn lại của đơn đang chọn
    private async Task NapDuLieuHienThi(ThanhToanCreateViewModel vm)
    {
        var cacDon = await _context.DonGiats
            .AsNoTracking()
            .Where(d => d.TrangThai != TrangThaiDonDaHuy)
            .Select(d => new DonCanThanhToanItem
            {
                MaDonGiat = d.MaDonGiat,
                TenKhachHang = d.KhachHang!.HoTen,
                TongTien = d.TongTienDuKien,
                DaThanhToan = _context.ThanhToans
                    .Where(t => t.MaDonGiat == d.MaDonGiat)
                    .Sum(t => (decimal?)t.SoTien) ?? 0m
            })
            .Where(x => x.TongTien > x.DaThanhToan)
            .OrderByDescending(x => x.MaDonGiat)
            .ToListAsync();

        vm.DanhSachDon = cacDon;

        var donDangChon = cacDon.FirstOrDefault(d => d.MaDonGiat == vm.MaDonGiat);
        if (donDangChon != null)
        {
            vm.TenKhachHang = donDangChon.TenKhachHang;
            vm.TongTienDon = donDangChon.TongTien;
            vm.TongDaThanhToan = donDangChon.DaThanhToan;
            vm.SoTienConLai = donDangChon.ConLai;
            vm.TrangThaiThanhToan = TrangThaiThanhToan.Tinh(donDangChon.TongTien, donDangChon.DaThanhToan);
        }
    }



    // GET: THANHTOANS/Edit/5
    public async Task<IActionResult> Edit(int? mathanhtoan)
    {
        if (mathanhtoan == null)
        {
            return NotFound();
        }

        var thanhtoan = await _context.ThanhToans.FindAsync(mathanhtoan);
        if (thanhtoan == null)
        {
            return NotFound();
        }
        return View(thanhtoan);
    }

    // POST: THANHTOANS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? mathanhtoan, [Bind("MaThanhToan,MaDonGiat,NgayThanhToan,SoTien,PhuongThuc,NguoiThucHien,GhiChu")] ThanhToan thanhtoan)
    {
        if (mathanhtoan != thanhtoan.MaThanhToan)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(thanhtoan);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ThanhToanExists(thanhtoan.MaThanhToan))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(thanhtoan);
    }

    // GET: THANHTOANS/Delete/5
    public async Task<IActionResult> Delete(int? mathanhtoan)
    {
        if (mathanhtoan == null)
        {
            return NotFound();
        }

        var thanhtoan = await _context.ThanhToans
            .FirstOrDefaultAsync(m => m.MaThanhToan == mathanhtoan);
        if (thanhtoan == null)
        {
            return NotFound();
        }

        return View(thanhtoan);
    }

    // POST: THANHTOANS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? mathanhtoan)
    {
        var thanhtoan = await _context.ThanhToans.FindAsync(mathanhtoan);
        if (thanhtoan != null)
        {
            _context.ThanhToans.Remove(thanhtoan);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ThanhToanExists(int? mathanhtoan)
    {
        return _context.ThanhToans.Any(e => e.MaThanhToan == mathanhtoan);
    }
}
