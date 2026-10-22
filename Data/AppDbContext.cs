// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: AppDbContext dùng chung cả nhóm: khai báo DbSet, khóa ngoại, ràng buộc không trùng (việc nhóm trưởng).
using Microsoft.EntityFrameworkCore;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TaiKhoan> TaiKhoans => Set<TaiKhoan>();
    public DbSet<LoaiDo> LoaiDos => Set<LoaiDo>();
    public DbSet<DonViTinh> DonViTinhs => Set<DonViTinh>();
    public DbSet<DichVu> DichVus => Set<DichVu>();
    public DbSet<BangGiaDichVu> BangGias => Set<BangGiaDichVu>();
    public DbSet<NhanVien> NhanViens => Set<NhanVien>();
    public DbSet<KhachHang> KhachHangs => Set<KhachHang>();
    public DbSet<DonGiat> DonGiats => Set<DonGiat>();
    public DbSet<ChiTietDonGiat> ChiTietDonGiats => Set<ChiTietDonGiat>();
    public DbSet<PhanCongXuLy> PhanCongs => Set<PhanCongXuLy>();
    public DbSet<CongDoanXuLy> CongDoans => Set<CongDoanXuLy>();
    public DbSet<GiaoTra> GiaoTras => Set<GiaoTra>();
    public DbSet<ThanhToan> ThanhToans => Set<ThanhToan>();
    public DbSet<KhieuNai> KhieuNais => Set<KhieuNai>();
    public DbSet<LichSuXuLy> LichSuXuLys => Set<LichSuXuLy>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<TaiKhoan>().HasIndex(x => x.TenDangNhap).IsUnique();
        mb.Entity<LoaiDo>().HasIndex(x => x.TenLoaiDo).IsUnique();
        mb.Entity<DonViTinh>().HasIndex(x => x.TenDonViTinh).IsUnique();
        mb.Entity<DichVu>().HasIndex(x => x.TenDichVu).IsUnique();
        mb.Entity<KhachHang>().HasIndex(x => x.SoDienThoai).IsUnique();

        mb.Entity<BangGiaDichVu>().Property(x => x.DonGia).HasPrecision(18, 2);
        mb.Entity<DonGiat>().Property(x => x.TongTienDuKien).HasPrecision(18, 2);
        mb.Entity<DonGiat>().Property(x => x.PhuPhi).HasPrecision(18, 2);
        mb.Entity<ChiTietDonGiat>().Property(x => x.SoLuongKhoiLuong).HasPrecision(18, 2);
        mb.Entity<ChiTietDonGiat>().Property(x => x.DonGiaApDung).HasPrecision(18, 2);
        mb.Entity<ChiTietDonGiat>().Property(x => x.ThanhTien).HasPrecision(18, 2);
        mb.Entity<ThanhToan>().Property(x => x.SoTien).HasPrecision(18, 2);

        // Đơn - Giao trả: 1-1
        mb.Entity<GiaoTra>().HasIndex(x => x.MaDonGiat).IsUnique();
        mb.Entity<DonGiat>().HasOne(d => d.GiaoTra).WithOne(g => g.DonGiat)
            .HasForeignKey<GiaoTra>(g => g.MaDonGiat);

        // ===== Khai báo rõ từng khóa ngoại (không để EF tự đoán) =====
        mb.Entity<BangGiaDichVu>().HasOne(b => b.DichVu).WithMany().HasForeignKey(b => b.MaDichVu);
        mb.Entity<BangGiaDichVu>().HasOne(b => b.LoaiDo).WithMany().HasForeignKey(b => b.MaLoaiDo);
        mb.Entity<BangGiaDichVu>().HasOne(b => b.DonViTinh).WithMany().HasForeignKey(b => b.MaDonViTinh);

        mb.Entity<NhanVien>().HasOne(n => n.TaiKhoan).WithMany().HasForeignKey(n => n.MaTaiKhoan);

        mb.Entity<DonGiat>().HasOne(d => d.KhachHang).WithMany(k => k.DonGiats).HasForeignKey(d => d.MaKhachHang);
        mb.Entity<DonGiat>().HasOne(d => d.NguoiTiepNhan).WithMany().HasForeignKey(d => d.MaNguoiTiepNhan);

        mb.Entity<ChiTietDonGiat>().HasOne(c => c.DonGiat).WithMany(d => d.ChiTiets).HasForeignKey(c => c.MaDonGiat);
        mb.Entity<ChiTietDonGiat>().HasOne(c => c.DichVu).WithMany().HasForeignKey(c => c.MaDichVu);
        mb.Entity<ChiTietDonGiat>().HasOne(c => c.LoaiDo).WithMany().HasForeignKey(c => c.MaLoaiDo);
        mb.Entity<ChiTietDonGiat>().HasOne(c => c.DonViTinh).WithMany().HasForeignKey(c => c.MaDonViTinh);

        mb.Entity<PhanCongXuLy>().HasOne(p => p.DonGiat).WithMany(d => d.PhanCongs).HasForeignKey(p => p.MaDonGiat);
        mb.Entity<PhanCongXuLy>().HasOne(p => p.NhanVien).WithMany().HasForeignKey(p => p.MaNhanVien);

        mb.Entity<CongDoanXuLy>().HasOne(c => c.DonGiat).WithMany(d => d.CongDoans).HasForeignKey(c => c.MaDonGiat);
        mb.Entity<CongDoanXuLy>().HasOne(c => c.NhanVien).WithMany().HasForeignKey(c => c.MaNhanVien);

        mb.Entity<ThanhToan>().HasOne(t => t.DonGiat).WithMany(d => d.ThanhToans).HasForeignKey(t => t.MaDonGiat);
        mb.Entity<KhieuNai>().HasOne(k => k.DonGiat).WithMany(d => d.KhieuNais).HasForeignKey(k => k.MaDonGiat);
        mb.Entity<LichSuXuLy>().HasOne(l => l.DonGiat).WithMany(d => d.LichSus).HasForeignKey(l => l.MaDonGiat);

        // Tránh lỗi "multiple cascade paths" của SQL Server
        foreach (var fk in mb.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            fk.DeleteBehavior = DeleteBehavior.Restrict;
    }
}
