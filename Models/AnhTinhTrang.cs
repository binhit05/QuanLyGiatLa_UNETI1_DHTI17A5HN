using System.ComponentModel.DataAnnotations;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Models
{
    public class AnhTinhTrang
    {
        [Key]
        public int MaAnh {  get; set; }

        //Anh thuoc dong do giat nao
        public int MaChiTiet { get; set; }
        public ChiTietDonGiat? ChiTietDonGiat { get; set; }

        //Đường dẫn ảnh lưu trong wwroot 
        [Required]
        [StringLength(500)]
        public string DuongDan {  get; set; } = string.Empty;

        // Tên file ảnh ban đầu 
        [StringLength(250)]
        public string? TenFile { get; set; }

        //Thời điểm tải ảnh
        public DateTime NgayTai { get; set; } = DateTime.Now;
    }
}
