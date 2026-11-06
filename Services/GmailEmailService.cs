using System;
using System.Net;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Options;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Services.Interfaces;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Services
{
    // Họ và tên: Lê Hoàng Anh
    // Mã sinh viên: 23103100214
    // Nội dung: Gửi email thông báo đơn giặt đã sẵn sàng giao trả.
    public class GmailEmailService : IEmailService
    {
        private readonly GmailSettings _gmailSettings;

        public GmailEmailService(IOptions<GmailSettings> gmailOptions)
        {
            _gmailSettings = gmailOptions.Value;
        }

        public async Task GuiThongBaoDonSanSangAsync(DonGiat donGiat)
        {
            // Kiểm tra thông tin đơn hàng và email khách hàng
            if (donGiat == null)
            {
                throw new ArgumentNullException(nameof(donGiat));
            }

            string emailKhachHang = donGiat.KhachHang?.Email;

            if (string.IsNullOrWhiteSpace(emailKhachHang))
            {
                throw new InvalidOperationException(
                    "Khách hàng chưa có địa chỉ email.");
            }

            if (string.IsNullOrWhiteSpace(_gmailSettings.Username) ||
                string.IsNullOrWhiteSpace(_gmailSettings.AppPassword))
            {
                throw new InvalidOperationException(
                    "Chưa cấu hình tài khoản Gmail hoặc mật khẩu ứng dụng.");
            }

            // Tạo nội dung email
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(
                _gmailSettings.SenderName,
                _gmailSettings.Username));

            message.To.Add(MailboxAddress.Parse(emailKhachHang));

            message.Subject = "Thông báo: Đơn giặt đã sẵn sàng giao trả";

            string maDon = WebUtility.HtmlEncode(
                donGiat.MaDonGiat.ToString());

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <div style='font-family: Arial, sans-serif;'>
                        <h2>Thông báo từ CleanGo</h2>
                        <p>Kính chào Quý khách,</p>
                        <p>Đơn giặt của Quý khách đã hoàn tất xử lý
                           và hiện đã sẵn sàng giao trả.</p>
                        <p><strong>Mã đơn giặt:</strong> {maDon}</p>
                        <p><strong>Trạng thái:</strong> Chờ giao trả</p>
                        <p>Quý khách vui lòng liên hệ cửa hàng
                           để nhận lại đồ.</p>
                        <p>Cảm ơn Quý khách đã sử dụng dịch vụ!</p>
                        <p><strong>CleanGo - Quản lý giặt là</strong></p>
                    </div>",

                TextBody =
                    $"Thông báo từ CleanGo\n" +
                    $"Đơn giặt mã {donGiat.MaDonGiat} đã hoàn tất xử lý " +
                    "và sẵn sàng giao trả.\n" +
                    "Quý khách vui lòng liên hệ cửa hàng để nhận lại đồ."
            };

            message.Body = bodyBuilder.ToMessageBody();

            // Kết nối Gmail SMTP và gửi email
            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(
                _gmailSettings.Host,
                _gmailSettings.Port,
                SecureSocketOptions.StartTls);

            await smtpClient.AuthenticateAsync(
                _gmailSettings.Username,
                _gmailSettings.AppPassword);

            await smtpClient.SendAsync(message);

            await smtpClient.DisconnectAsync(true);
        }
    }
}
