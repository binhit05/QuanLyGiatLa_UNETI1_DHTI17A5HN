using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
using System;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Services
{
    public class CongDoanXuLyService
    {
        //Bắt đầu một công đoạn 
        public bool BatDau(CongDoanXuLy congDoan, out string thongBao)
        {
            if(congDoan.TrangThai != TrangThaiCongDoan.ChuaBatDau)
            {
                thongBao = "Chỉ có thể bắt đầu công đoạn chưa bắt đầu.";
                return false;
            }
            if(congDoan.ThoiGianBatDau.HasValue || congDoan.ThoiGianKetThuc.HasValue)
            {
                thongBao = "Dữ liệu thời gian công đoạn không hợp lệ.";
                return false;
            }

            congDoan.ThoiGianBatDau = DateTime.Now;
            congDoan.TrangThai = TrangThaiCongDoan.DangXuLy;

            thongBao = "Bắt đầu công đoạn thành công.";
            return true;
        }

        //Hoàn thành một công đoạn 
        public bool HoanThanh(CongDoanXuLy congDoan, out string thongBao)
        {
            if(congDoan.TrangThai != TrangThaiCongDoan.DangXuLy)
            {
                thongBao = "Chỉ có thể hoàn thành công đoạn đang xử lý.";
                return false;
            }
            if (!congDoan.ThoiGianBatDau.HasValue)
            {
                thongBao = "Công đoạn chưa có thời gian bắt đầu.";
                return false;
            }

            DateTime thoiDiemKetThuc = DateTime.Now;

            if (thoiDiemKetThuc < congDoan.ThoiGianBatDau.Value)
            {
                thongBao = "Thời gian kết thúc không hợp lệ.";
                return false;
            }

            congDoan.ThoiGianKetThuc = thoiDiemKetThuc;
            congDoan.TrangThai = TrangThaiCongDoan.HoanThanh;

            thongBao = "Hoàn thành công đoạn thành công.";
            return true;
        }

        // Hủy một công đoạn
        public bool Huy(CongDoanXuLy congDoan, out string thongBao)
        {
            if (congDoan.TrangThai == TrangThaiCongDoan.HoanThanh)
            {
                thongBao = "Không thể hủy công đoạn đã hoàn thành.";
                return false;
            }

            if (congDoan.TrangThai == TrangThaiCongDoan.DaHuy)
            {
                thongBao = "Công đoạn đã được hủy trước đó.";
                return false;
            }

            congDoan.TrangThai = TrangThaiCongDoan.DaHuy;

            thongBao = "Hủy công đoạn thành công.";
            return true;
        }
    }
}
