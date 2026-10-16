using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Services
{
    public class GiaoTraService
    {

        public bool KiemTraDuLieu(GiaoTra giaoTra, out string thongBao)
        {
            if (giaoTra == null)
            {
                thongBao = "Thông tin giao trả không được để trống.";
                return false;
            }

            if (giaoTra.MaDonGiat <= 0)
            {
                thongBao = "Mã đơn giặt không hợp lệ.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(giaoTra.NguoiGiao))
            {
                thongBao = "Người giao là bắt buộc.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(giaoTra.NguoiNhan))
            {
                thongBao = "Người nhận là bắt buộc.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(giaoTra.PhuongThucGiaoTra))
            {
                thongBao = "Phương thức giao trả là bắt buộc.";
                return false;
            }

            if (giaoTra.NguoiGiao.Length > 100)
            {
                thongBao = "Tên người giao tối đa 100 ký tự.";
                return false;
            }

            if (giaoTra.NguoiNhan.Length > 100)
            {
                thongBao = "Tên người nhận tối đa 100 ký tự.";
                return false;
            }

            if (giaoTra.PhuongThucGiaoTra.Length > 50)
            {
                thongBao = "Phương thức giao trả tối đa 50 ký tự.";
                return false;
            }

            if (giaoTra.GhiChu != null && giaoTra.GhiChu.Length > 500)
            {
                thongBao = "Ghi chú tối đa 500 ký tự.";
                return false;
            }

            if (giaoTra.NgayGiaoTra == default)
            {
                thongBao = "Ngày giao trả không hợp lệ.";
                return false;
            }

            thongBao = "Dữ liệu giao trả hợp lệ.";
            return true;
        }

        public bool KiemTraNgayGiaoTra(
            DateTime ngayGiaoTra,
            out string thongBao)
        {
            if (ngayGiaoTra == default)
            {
                thongBao = "Ngày giao trả không hợp lệ.";
                return false;
            }

            if (ngayGiaoTra > DateTime.Now)
            {
                thongBao = "Ngày giao trả không được ở tương lai.";
                return false;
            }

            thongBao = "Ngày giao trả hợp lệ.";
            return true;
        }

    }
}
