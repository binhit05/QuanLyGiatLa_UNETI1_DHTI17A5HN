// Họ và tên: Nghiêm Xuân Bằng
// Mã sinh viên: .....................
// Nội dung thực hiện: Phân trang bằng EF Core/LINQ (Skip/Take) dùng chung.
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;

public interface IPaged
{
    int Page { get; }
    int PageSize { get; }
    int TotalPages { get; }
    int TotalItems { get; }
    int CountOnPage { get; }
}

public class PagedList<T> : IPaged
{
    public List<T> Items { get; private set; } = new();
    public int Page { get; private set; }
    public int PageSize { get; private set; }
    public int TotalItems { get; private set; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalItems / (double)PageSize));
    public int CountOnPage => Items.Count;

    // Tạo từ danh sách đã có sẵn (giữ thông tin phân trang của danh sách gốc)
    public static PagedList<T> TuDanhSach(List<T> items, int page, int pageSize, int total)
    {
        return new PagedList<T> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public static PagedList<T> Tao(IQueryable<T> query, int page, int pageSize)
    {
        var total = query.Count();
        var pages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));
        page = Math.Clamp(page, 1, pages);
        return new PagedList<T>
        {
            Page = page,
            PageSize = pageSize,
            TotalItems = total,
            Items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList()
        };
    }
}
