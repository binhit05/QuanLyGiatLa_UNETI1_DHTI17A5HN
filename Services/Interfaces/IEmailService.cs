using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using System.Threading.Tasks;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Services.Interfaces
{
    // Họ và tên: Lê Hoàng Anh
    // Mã sinh viên: 23103100214 
    // Nội dung: Định nghĩa chức năng gửi email thông báo đơn giặt sẵn sàng giao trả.
    public interface IEmailService
    {
        Task GuiThongBaoDonSanSangAsync(DonGiat donGiat);
    }
}
