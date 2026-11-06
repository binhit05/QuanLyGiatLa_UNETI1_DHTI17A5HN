using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Migrations
{
    /// <inheritdoc />
    public partial class AddAnhTinhTrang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnhTinhTrangs",
                columns: table => new
                {
                    MaAnh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaChiTiet = table.Column<int>(type: "int", nullable: false),
                    DuongDan = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TenFile = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    NgayTai = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnhTinhTrangs", x => x.MaAnh);
                    table.ForeignKey(
                        name: "FK_AnhTinhTrangs_ChiTietDonGiats_MaChiTiet",
                        column: x => x.MaChiTiet,
                        principalTable: "ChiTietDonGiats",
                        principalColumn: "MaChiTiet",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnhTinhTrangs_MaChiTiet",
                table: "AnhTinhTrangs",
                column: "MaChiTiet");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnhTinhTrangs");
        }
    }
}
