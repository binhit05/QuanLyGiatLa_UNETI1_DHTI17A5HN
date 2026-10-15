// Họ và tên: Lê Duy Khánh
// Mã sinh viên: 23103100216
//Nội dung thực hiện: Danh sách thanh toán (Search/Filter/Sort/Pagination)
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.ViewModels
{
    public class ThanhToanListViewModel
    {
        public List<ThanhToan> DanhSach { get; set; } = new();

        //Tìm kiếm/lọc/sắp xếp(giữ lại khi chuyển trang)
        public string? TuKhoa { get; set; }
        public string? PhuongThuc {  get; set; }
        public DateTime? TuNgay {  get; set; }
        public DateTime? DenNgay {  get; set; }
        public string? SapXep {  get; set; }

        //Phân trang
        public int TrangHienTai { get; set; } = 1;
        public int KichThuocTrang { get; set; } = 10;
        public int TongSoBanGhi { get; set; }
        public int TongSoTrang { get; set; }
        public bool CoTrangTruoc => TrangHienTai > 1;
        public bool CoTrangSau => TrangHienTai < TongSoTrang;

        //Tổng tiền của toàn bộ sau khi lọc(Không tính một trang duy nhất)
        public decimal TongSoTienLoc {  get; set; }
    }
}
