using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Migrations
{
    /// <inheritdoc />
    public partial class QLGiatLaDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DichVus",
                columns: table => new
                {
                    MaDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ThoiGianXuLyDuKien = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVus", x => x.MaDichVu);
                });

            migrationBuilder.CreateTable(
                name: "DonViTinhs",
                columns: table => new
                {
                    MaDonViTinh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDonViTinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonViTinhs", x => x.MaDonViTinh);
                });

            migrationBuilder.CreateTable(
                name: "KhachHangs",
                columns: table => new
                {
                    MaKhachHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHangs", x => x.MaKhachHang);
                });

            migrationBuilder.CreateTable(
                name: "LoaiDos",
                columns: table => new
                {
                    MaLoaiDo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoaiDo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiDos", x => x.MaLoaiDo);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VaiTro = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false),
                    LanDangNhapCuoi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "BangGias",
                columns: table => new
                {
                    MaBangGia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    MaLoaiDo = table.Column<int>(type: "int", nullable: false),
                    MaDonViTinh = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NgayApDung = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangGias", x => x.MaBangGia);
                    table.ForeignKey(
                        name: "FK_BangGias_DichVus_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVus",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BangGias_DonViTinhs_MaDonViTinh",
                        column: x => x.MaDonViTinh,
                        principalTable: "DonViTinhs",
                        principalColumn: "MaDonViTinh",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BangGias_LoaiDos_MaLoaiDo",
                        column: x => x.MaLoaiDo,
                        principalTable: "LoaiDos",
                        principalColumn: "MaLoaiDo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DonGiats",
                columns: table => new
                {
                    MaDonGiat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    NgayTiepNhan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayHenTra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaNguoiTiepNhan = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    PhuPhi = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TongTienDuKien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonGiats", x => x.MaDonGiat);
                    table.ForeignKey(
                        name: "FK_DonGiats_KhachHangs_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonGiats_TaiKhoans_MaNguoiTiepNhan",
                        column: x => x.MaNguoiTiepNhan,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NhanViens",
                columns: table => new
                {
                    MaNhanVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChucVu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanViens", x => x.MaNhanVien);
                    table.ForeignKey(
                        name: "FK_NhanViens_TaiKhoans_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonGiats",
                columns: table => new
                {
                    MaChiTiet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonGiat = table.Column<int>(type: "int", nullable: false),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    MaLoaiDo = table.Column<int>(type: "int", nullable: false),
                    MaDonViTinh = table.Column<int>(type: "int", nullable: false),
                    SoLuongKhoiLuong = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DonGiaApDung = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TinhTrangLucNhan = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    YeuCauDacBiet = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonGiats", x => x.MaChiTiet);
                    table.ForeignKey(
                        name: "FK_ChiTietDonGiats_DichVus_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVus",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietDonGiats_DonGiats_MaDonGiat",
                        column: x => x.MaDonGiat,
                        principalTable: "DonGiats",
                        principalColumn: "MaDonGiat",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietDonGiats_DonViTinhs_MaDonViTinh",
                        column: x => x.MaDonViTinh,
                        principalTable: "DonViTinhs",
                        principalColumn: "MaDonViTinh",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietDonGiats_LoaiDos_MaLoaiDo",
                        column: x => x.MaLoaiDo,
                        principalTable: "LoaiDos",
                        principalColumn: "MaLoaiDo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GiaoTras",
                columns: table => new
                {
                    MaGiaoTra = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonGiat = table.Column<int>(type: "int", nullable: false),
                    NgayGiaoTra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiGiao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NguoiNhan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HinhThucNhan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaoTras", x => x.MaGiaoTra);
                    table.ForeignKey(
                        name: "FK_GiaoTras_DonGiats_MaDonGiat",
                        column: x => x.MaDonGiat,
                        principalTable: "DonGiats",
                        principalColumn: "MaDonGiat",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KhieuNais",
                columns: table => new
                {
                    MaKhieuNai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonGiat = table.Column<int>(type: "int", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MucDo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NguoiPhuTrach = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HuongXuLy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NgayHoanTat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhieuNais", x => x.MaKhieuNai);
                    table.ForeignKey(
                        name: "FK_KhieuNais_DonGiats_MaDonGiat",
                        column: x => x.MaDonGiat,
                        principalTable: "DonGiats",
                        principalColumn: "MaDonGiat",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichSuXuLys",
                columns: table => new
                {
                    MaLichSu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonGiat = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NguoiThucHien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuXuLys", x => x.MaLichSu);
                    table.ForeignKey(
                        name: "FK_LichSuXuLys_DonGiats_MaDonGiat",
                        column: x => x.MaDonGiat,
                        principalTable: "DonGiats",
                        principalColumn: "MaDonGiat",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThanhToans",
                columns: table => new
                {
                    MaThanhToan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonGiat = table.Column<int>(type: "int", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PhuongThuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NguoiThucHien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToans", x => x.MaThanhToan);
                    table.ForeignKey(
                        name: "FK_ThanhToans_DonGiats_MaDonGiat",
                        column: x => x.MaDonGiat,
                        principalTable: "DonGiats",
                        principalColumn: "MaDonGiat",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CongDoans",
                columns: table => new
                {
                    MaCongDoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonGiat = table.Column<int>(type: "int", nullable: false),
                    TenCongDoan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThuTu = table.Column<int>(type: "int", nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaNhanVien = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CongDoans", x => x.MaCongDoan);
                    table.ForeignKey(
                        name: "FK_CongDoans_DonGiats_MaDonGiat",
                        column: x => x.MaDonGiat,
                        principalTable: "DonGiats",
                        principalColumn: "MaDonGiat",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CongDoans_NhanViens_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanViens",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PhanCongs",
                columns: table => new
                {
                    MaPhanCong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonGiat = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    NgayPhanCong = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NoiDungCongViec = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanCongs", x => x.MaPhanCong);
                    table.ForeignKey(
                        name: "FK_PhanCongs_DonGiats_MaDonGiat",
                        column: x => x.MaDonGiat,
                        principalTable: "DonGiats",
                        principalColumn: "MaDonGiat",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhanCongs_NhanViens_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanViens",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BangGias_MaDichVu",
                table: "BangGias",
                column: "MaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_BangGias_MaDonViTinh",
                table: "BangGias",
                column: "MaDonViTinh");

            migrationBuilder.CreateIndex(
                name: "IX_BangGias_MaLoaiDo",
                table: "BangGias",
                column: "MaLoaiDo");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonGiats_MaDichVu",
                table: "ChiTietDonGiats",
                column: "MaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonGiats_MaDonGiat",
                table: "ChiTietDonGiats",
                column: "MaDonGiat");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonGiats_MaDonViTinh",
                table: "ChiTietDonGiats",
                column: "MaDonViTinh");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonGiats_MaLoaiDo",
                table: "ChiTietDonGiats",
                column: "MaLoaiDo");

            migrationBuilder.CreateIndex(
                name: "IX_CongDoans_MaDonGiat",
                table: "CongDoans",
                column: "MaDonGiat");

            migrationBuilder.CreateIndex(
                name: "IX_CongDoans_MaNhanVien",
                table: "CongDoans",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_DichVus_TenDichVu",
                table: "DichVus",
                column: "TenDichVu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonGiats_MaKhachHang",
                table: "DonGiats",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonGiats_MaNguoiTiepNhan",
                table: "DonGiats",
                column: "MaNguoiTiepNhan");

            migrationBuilder.CreateIndex(
                name: "IX_DonViTinhs_TenDonViTinh",
                table: "DonViTinhs",
                column: "TenDonViTinh",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GiaoTras_MaDonGiat",
                table: "GiaoTras",
                column: "MaDonGiat",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_SoDienThoai",
                table: "KhachHangs",
                column: "SoDienThoai",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhieuNais_MaDonGiat",
                table: "KhieuNais",
                column: "MaDonGiat");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuXuLys_MaDonGiat",
                table: "LichSuXuLys",
                column: "MaDonGiat");

            migrationBuilder.CreateIndex(
                name: "IX_LoaiDos_TenLoaiDo",
                table: "LoaiDos",
                column: "TenLoaiDo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhanViens_MaTaiKhoan",
                table: "NhanViens",
                column: "MaTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongs_MaDonGiat",
                table: "PhanCongs",
                column: "MaDonGiat");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCongs_MaNhanVien",
                table: "PhanCongs",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_TenDangNhap",
                table: "TaiKhoans",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToans_MaDonGiat",
                table: "ThanhToans",
                column: "MaDonGiat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BangGias");

            migrationBuilder.DropTable(
                name: "ChiTietDonGiats");

            migrationBuilder.DropTable(
                name: "CongDoans");

            migrationBuilder.DropTable(
                name: "GiaoTras");

            migrationBuilder.DropTable(
                name: "KhieuNais");

            migrationBuilder.DropTable(
                name: "LichSuXuLys");

            migrationBuilder.DropTable(
                name: "PhanCongs");

            migrationBuilder.DropTable(
                name: "ThanhToans");

            migrationBuilder.DropTable(
                name: "DichVus");

            migrationBuilder.DropTable(
                name: "DonViTinhs");

            migrationBuilder.DropTable(
                name: "LoaiDos");

            migrationBuilder.DropTable(
                name: "NhanViens");

            migrationBuilder.DropTable(
                name: "DonGiats");

            migrationBuilder.DropTable(
                name: "KhachHangs");

            migrationBuilder.DropTable(
                name: "TaiKhoans");
        }
    }
}
