namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Options
{
    //Họ tên: Lê Hoàng Anh
    //MSV: 23103100214
    //Nội dung thực hiện: Cấu hình thông tin gmail SMTP để gửi email

    public class GmailSettings
    {
        //Máy chủ SMTP Gmail
        public string Host { get; set; } = "smtp.gmail.com";
        //Cổng SMTP sử dụng STARTTLS 
        public int Port { get; set; } = 587;

        // Địa chỉ Gmail gửi thông báo
        public string Username { get; set; } = string.Empty;

        // Mật khẩu ứng dụng Gmail
        public string AppPassword { get; set; } = string.Empty;

        // Tên hiển thị của người gửi
        public string SenderName { get; set; } = "CleanGo - Quản lý giặt là";
    }
}
