// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: AppDbContext dùng chung cả nhóm, khai báo DbSet và ràng buộc không trùng cho Module 1.

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