using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Services
{
    public class PhanCongXuLyService
    {
        // Kiểm tra dữ liệu và trạng thái khi tạo phân công.
        public bool KiemTraPhanCong(
            PhanCongXuLy phanCong,
            out string thongBao)
        {
            if (phanCong.MaDonGiat <= 0)
            {
                thongBao = "Mã đơn giặt không hợp lệ.";
                return false;
            }

            if (phanCong.MaNhanVien <= 0)
            {
                thongBao = "Mã nhân viên không hợp lệ.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(phanCong.NoiDungCongViec))
            {
                thongBao = "Nội dung công việc là bắt buộc.";
                return false;
            }

            if (phanCong.NoiDungCongViec.Length > 300)
            {
                thongBao = "Nội dung công việc tối đa 300 ký tự.";
                return false;
            }

            if (!TrangThaiPhanCong.HopLe(phanCong.TrangThai))
            {
                thongBao = "Trạng thái phân công không hợp lệ.";
                return false;
            }

            thongBao = "Dữ liệu phân công hợp lệ.";
            return true;
        }

        // Bắt đầu thực hiện một phân công.
        public bool BatDau(
            PhanCongXuLy phanCong,
            out string thongBao)
        {
            if (phanCong.TrangThai != TrangThaiPhanCong.ChuaBatDau)
            {
                thongBao = "Chỉ có thể bắt đầu phân công chưa bắt đầu.";
                return false;
            }

            phanCong.TrangThai = TrangThaiPhanCong.DangXuLy;

            thongBao = "Bắt đầu phân công thành công.";
            return true;
        }

        // Hoàn thành một phân công.
        public bool HoanThanh(
            PhanCongXuLy phanCong,
            out string thongBao)
        {
            if (phanCong.TrangThai != TrangThaiPhanCong.DangXuLy)
            {
                thongBao = "Chỉ có thể hoàn thành phân công đang xử lý.";
                return false;
            }

            phanCong.TrangThai = TrangThaiPhanCong.HoanThanh;

            thongBao = "Hoàn thành phân công thành công.";
            return true;
        }

        // Hủy một phân công.
        public bool Huy(
            PhanCongXuLy phanCong,
            out string thongBao)
        {
            if (phanCong.TrangThai == TrangThaiPhanCong.HoanThanh)
            {
                thongBao = "Không thể hủy phân công đã hoàn thành.";
                return false;
            }

            if (phanCong.TrangThai == TrangThaiPhanCong.DaHuy)
            {
                thongBao = "Phân công đã được hủy trước đó.";
                return false;
            }

            phanCong.TrangThai = TrangThaiPhanCong.DaHuy;

            thongBao = "Hủy phân công thành công.";
            return true;
        }
    }
}
