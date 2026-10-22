// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: Quản lý danh mục dùng chung: loại đồ và đơn vị tính.
using Microsoft.AspNetCore.Mvc;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Filters;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Controllers;

// Danh mục dùng chung: Loại đồ + Đơn vị tính (2 tab)
[PhanQuyen(VaiTroConst.Admin)]
public class DanhMucController : BaseController
{
    public DanhMucController(AppDbContext db) : base(db) { }

    public IActionResult Index(string? tab, string? q, string? trangThai, string? sort, int page = 1)
    {
        tab = tab == "donvitinh" ? "donvitinh" : "loaido";
        ViewBag.Tab = tab;
        ViewBag.SoLoaiDo = _db.LoaiDos.Count();
        ViewBag.SoDonViTinh = _db.DonViTinhs.Count();

        PagedList<DanhMucRow> kq;
        if (tab == "loaido")
        {
            var query = _db.LoaiDos.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q)) { var s = q.Trim(); query = query.Where(x => x.TenLoaiDo.Contains(s) || (x.MoTa != null && x.MoTa.Contains(s))); }
            if (trangThai == "1") query = query.Where(x => x.TrangThai); else if (trangThai == "0") query = query.Where(x => !x.TrangThai);
            query = sort == "ten" ? query.OrderBy(x => x.TenLoaiDo) : sort == "cu" ? query.OrderBy(x => x.MaLoaiDo) : query.OrderByDescending(x => x.MaLoaiDo);
            var ds = PagedList<LoaiDo>.Tao(query, page, CoTrang);
            var ids = ds.Items.Select(x => x.MaLoaiDo).ToList();
            var dem = _db.BangGias.Where(b => ids.Contains(b.MaLoaiDo)).GroupBy(b => b.MaLoaiDo)
                        .Select(g => new { g.Key, Dem = g.Count() }).ToDictionary(x => x.Key, x => x.Dem);
            kq = Doi(ds, ds.Items.Select(x => new DanhMucRow { Id = x.MaLoaiDo, Ma = UiHelper.MaDM("LD", x.MaLoaiDo), Ten = x.TenLoaiDo, MoTa = x.MoTa, TrangThai = x.TrangThai, DangSuDung = dem.ContainsKey(x.MaLoaiDo) ? dem[x.MaLoaiDo] : 0 }).ToList());
        }
        else
        {
            var query = _db.DonViTinhs.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q)) { var s = q.Trim(); query = query.Where(x => x.TenDonViTinh.Contains(s) || (x.MoTa != null && x.MoTa.Contains(s))); }
            if (trangThai == "1") query = query.Where(x => x.TrangThai); else if (trangThai == "0") query = query.Where(x => !x.TrangThai);
            query = sort == "ten" ? query.OrderBy(x => x.TenDonViTinh) : sort == "cu" ? query.OrderBy(x => x.MaDonViTinh) : query.OrderByDescending(x => x.MaDonViTinh);
            var ds = PagedList<DonViTinh>.Tao(query, page, CoTrang);
            var ids = ds.Items.Select(x => x.MaDonViTinh).ToList();
            var dem = _db.BangGias.Where(b => ids.Contains(b.MaDonViTinh)).GroupBy(b => b.MaDonViTinh)
                        .Select(g => new { g.Key, Dem = g.Count() }).ToDictionary(x => x.Key, x => x.Dem);
            kq = Doi(ds, ds.Items.Select(x => new DanhMucRow { Id = x.MaDonViTinh, Ma = UiHelper.MaDM("DT", x.MaDonViTinh), Ten = x.TenDonViTinh, MoTa = x.MoTa, TrangThai = x.TrangThai, DangSuDung = dem.ContainsKey(x.MaDonViTinh) ? dem[x.MaDonViTinh] : 0 }).ToList());
        }

        ViewBag.Filter = new FilterBarVM
        {
            Placeholder = "Tìm kiếm theo tên, mô tả...", Q = q, Sort = sort ?? "moi",
            Selects = new List<SelectBox> { new SelectBox { Name = "trangThai", AllText = "Tất cả trạng thái", Selected = trangThai,
                Opts = new List<Opt> { new Opt("1", "Hoạt động"), new Opt("0", "Ngừng hoạt động") } } },
            Hidden = new Dictionary<string, string> { { "tab", tab } }
        };
        return View(kq);
    }

    // chuyển PagedList<T> sang PagedList<DanhMucRow> giữ nguyên thông tin phân trang
    private static PagedList<DanhMucRow> Doi<T>(PagedList<T> goc, List<DanhMucRow> rows)
    {
        return PagedList<DanhMucRow>.TuDanhSach(rows, goc.Page, goc.PageSize, goc.TotalItems);
    }

    [HttpGet]
    public IActionResult Create(string? tab) => View("Form", new DanhMucForm { Tab = tab == "donvitinh" ? "donvitinh" : "loaido" });

    [HttpPost]
    public IActionResult Create(DanhMucForm vm)
    {
        bool trung = vm.Tab == "loaido" ? _db.LoaiDos.Any(x => x.TenLoaiDo == vm.Ten) : _db.DonViTinhs.Any(x => x.TenDonViTinh == vm.Ten);
        if (trung) ModelState.AddModelError("Ten", "Tên đã tồn tại");
        if (!ModelState.IsValid) return View("Form", vm);

        if (vm.Tab == "loaido") _db.LoaiDos.Add(new LoaiDo { TenLoaiDo = vm.Ten.Trim(), MoTa = vm.MoTa, TrangThai = vm.TrangThai });
        else _db.DonViTinhs.Add(new DonViTinh { TenDonViTinh = vm.Ten.Trim(), MoTa = vm.MoTa, TrangThai = vm.TrangThai });
        _db.SaveChanges();
        ThongBao("Thêm danh mục thành công");
        return RedirectToAction(nameof(Index), new { tab = vm.Tab });
    }

    [HttpGet]
    public IActionResult Edit(string tab, int id)
    {
        if (tab == "donvitinh")
        {
            var x = _db.DonViTinhs.Find(id); if (x == null) return NotFound();
            return View("Form", new DanhMucForm { Id = x.MaDonViTinh, Tab = tab, Ten = x.TenDonViTinh, MoTa = x.MoTa, TrangThai = x.TrangThai });
        }
        var l = _db.LoaiDos.Find(id); if (l == null) return NotFound();
        return View("Form", new DanhMucForm { Id = l.MaLoaiDo, Tab = "loaido", Ten = l.TenLoaiDo, MoTa = l.MoTa, TrangThai = l.TrangThai });
    }

    [HttpPost]
    public IActionResult Edit(DanhMucForm vm)
    {
        bool trung = vm.Tab == "loaido" ? _db.LoaiDos.Any(x => x.TenLoaiDo == vm.Ten && x.MaLoaiDo != vm.Id)
                                         : _db.DonViTinhs.Any(x => x.TenDonViTinh == vm.Ten && x.MaDonViTinh != vm.Id);
        if (trung) ModelState.AddModelError("Ten", "Tên đã tồn tại");
        if (!ModelState.IsValid) return View("Form", vm);

        if (vm.Tab == "loaido")
        {
            var x = _db.LoaiDos.Find(vm.Id); if (x == null) return NotFound();
            x.TenLoaiDo = vm.Ten.Trim(); x.MoTa = vm.MoTa; x.TrangThai = vm.TrangThai;
        }
        else
        {
            var x = _db.DonViTinhs.Find(vm.Id); if (x == null) return NotFound();
            x.TenDonViTinh = vm.Ten.Trim(); x.MoTa = vm.MoTa; x.TrangThai = vm.TrangThai;
        }
        _db.SaveChanges();
        ThongBao("Cập nhật thành công");
        return RedirectToAction(nameof(Index), new { tab = vm.Tab });
    }

    [HttpPost]
    public IActionResult Delete(string tab, int id)
    {
        if (tab == "donvitinh")
        {
            var x = _db.DonViTinhs.Find(id); if (x == null) return NotFound();
            if (_db.BangGias.Any(b => b.MaDonViTinh == id) || _db.ChiTietDonGiats.Any(c => c.MaDonViTinh == id))
            { BaoLoi("Đơn vị tính đang được sử dụng nên không thể xóa. Hãy chuyển sang ngừng hoạt động."); return RedirectToAction(nameof(Index), new { tab }); }
            _db.DonViTinhs.Remove(x);
        }
        else
        {
            var x = _db.LoaiDos.Find(id); if (x == null) return NotFound();
            if (_db.BangGias.Any(b => b.MaLoaiDo == id) || _db.ChiTietDonGiats.Any(c => c.MaLoaiDo == id))
            { BaoLoi("Loại đồ đang được sử dụng nên không thể xóa. Hãy chuyển sang ngừng hoạt động."); return RedirectToAction(nameof(Index), new { tab = "loaido" }); }
            _db.LoaiDos.Remove(x);
        }
        _db.SaveChanges();
        ThongBao("Đã xóa danh mục");
        return RedirectToAction(nameof(Index), new { tab });
    }
}
