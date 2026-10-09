using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models
{
    public class BangGiaDichVu
    {
        [Key]
        public int MaBangGia { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn dịch vụ")]
        public int MaDichVu { get; set; }

        [ForeignKey("MaDichVu")]
        public DichVu? DichVu { get; set; }

        [Required(ErrorMessage = "Đơn giá không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn 0")]
        public decimal DonGia { get; set; }

        [Required(ErrorMessage = "Ngày áp dụng không được để trống")]
        [DataType(DataType.Date)]
        public DateTime NgayApDung { get; set; }

        [DataType(DataType.Date)]
        public DateTime? NgayKetThuc { get; set; }

        public bool TrangThai { get; set; } = true;
    }
}
