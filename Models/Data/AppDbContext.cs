// Họ và tên: Trần Văn Bình - Nghiêm Xuân Bằng
// Mã sinh viên: 23103100179 - [Điền MSV của bạn]
// Nội dung thực hiện: AppDbContext dùng chung cả nhóm, khai báo DbSet cho Module 1 & Module 2.

using Microsoft.EntityFrameworkCore;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // ===== MODULE 1 - Trần Văn Bình =====
    public DbSet<TaiKhoan> TaiKhoans { get; set; }
    public DbSet<LoaiDo> LoaiDos { get; set; }
    public DbSet<DonViTinh> DonViTinhs { get; set; }

    // ===== MODULE 2 - Nghiêm Xuân Bằng =====
    public DbSet<DichVu> DichVu { get; set; }
    public DbSet<BangGiaDichVu> BangGiaDichVu { get; set; }
    public DbSet<NhanVien> NhanVien { get; set; }

    // ===== MODULE 3 - Lê Trường Giang =====

    // ===== MODULE 4 - Lê Hoàng Anh =====

    // ===== MODULE 5 - Lê Duy Khánh =====

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Module 1 - Trần Văn Bình
        modelBuilder.Entity<TaiKhoan>().HasIndex(t => t.TenDangNhap).IsUnique();
        modelBuilder.Entity<LoaiDo>().HasIndex(l => l.TenLoaiDo).IsUnique();
        modelBuilder.Entity<DonViTinh>().HasIndex(d => d.TenDonViTinh).IsUnique();

        // Module 2 - Nghiêm Xuân Bằng

        // Module 3 - Lê Trường Giang

        // Module 4 - Lê Hoàng Anh

        // Module 5 - Lê Duy Khánh
    }
}