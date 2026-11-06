// Họ và tên: Nghiêm Xuân Bằng
// Mã sinh viên: .....................
// Nội dung thực hiện: Mô hình thanh tìm kiếm/lọc/sắp xếp dùng chung.
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels;

public record Opt(string Value, string Text);

public class SelectBox
{
    public string Name { get; set; } = "";
    public string AllText { get; set; } = "";
    public List<Opt> Opts { get; set; } = new();
    public string? Selected { get; set; }
}

public class FilterBarVM
{
    public string Placeholder { get; set; } = "Tìm kiếm...";
    public string? Q { get; set; }
    public List<SelectBox> Selects { get; set; } = new();
    public string? Sort { get; set; }
    public List<Opt> Sorts { get; set; } = new List<Opt> { new Opt("moi", "Mới nhất"), new Opt("cu", "Cũ nhất"), new Opt("ten", "Tên A → Z") };
    public bool ShowDate { get; set; }
    public string? Ngay { get; set; }
    public Dictionary<string, string> Hidden { get; set; } = new();
}
