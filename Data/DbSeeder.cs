// Họ và tên: Trần Văn Bình
// Mã sinh viên: 23103100179
// Nội dung thực hiện: Dữ liệu mẫu dùng chung: tài khoản, nhân viên, khách hàng, đơn giặt và các bảng liên quan (việc nhóm trưởng).
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Helpers;
using QuanLyGiatLa_UNETI1_DHTI17A5HN.Models;
namespace QuanLyGiatLa_UNETI1_DHTI17A5HN.Data;

// Dữ liệu mẫu: 6 tài khoản, 5 nhân viên, 38 khách hàng, 60 đơn giặt kèm chi tiết,
// phân công, công đoạn, thanh toán, giao trả, khiếu nại, lịch sử.
public static class DbSeeder
{
    public const string SoDienThoaiLienLac = "0987188328";

    public static void Seed(AppDbContext db)
    {
        if (db.TaiKhoans.Any()) return;
        var rnd = new Random(2026);
        var now = DateTime.Now;
        DateTime Cap(DateTime x) => x > now ? now.AddMinutes(-1) : x;

        // ===== Tài khoản (mật khẩu 123456) =====
        var tk = new List<TaiKhoan>
        {
            new TaiKhoan { TenDangNhap = "admin",     MatKhau = "123456", HoTen = "Trần Văn Bình",    Email = "binh.tv@cleango.vn",     VaiTro = VaiTroConst.Admin,            LanDangNhapCuoi = now.AddMinutes(-35) },
            new TaiKhoan { TenDangNhap = "tiepnhan",  MatKhau = "123456", HoTen = "Nghiêm Xuân Bằng", Email = "bang.nx@cleango.vn",     VaiTro = VaiTroConst.NhanVienTiepNhan, LanDangNhapCuoi = now.AddHours(-3) },
            new TaiKhoan { TenDangNhap = "tiepnhan2", MatKhau = "123456", HoTen = "Lê Trường Giang",  Email = "giang.lt@cleango.vn",    VaiTro = VaiTroConst.NhanVienTiepNhan, LanDangNhapCuoi = now.AddDays(-1) },
            new TaiKhoan { TenDangNhap = "xuly",      MatKhau = "123456", HoTen = "Lê Hoàng Anh",     Email = "hoanganh.l@cleango.vn",  VaiTro = VaiTroConst.NhanVienXuLy,     LanDangNhapCuoi = now.AddHours(-5) },
            new TaiKhoan { TenDangNhap = "xuly2",     MatKhau = "123456", HoTen = "Lê Duy Khánh",     Email = "khanh.ld@cleango.vn",    VaiTro = VaiTroConst.NhanVienXuLy,     LanDangNhapCuoi = now.AddDays(-2) },
            new TaiKhoan { TenDangNhap = "khoa",      MatKhau = "123456", HoTen = "Tài khoản khóa (test)", Email = "khoa@cleango.vn",   VaiTro = VaiTroConst.NhanVienXuLy,     TrangThai = false, LanDangNhapCuoi = now.AddDays(-9) },
        };
        db.TaiKhoans.AddRange(tk);
        db.SaveChanges();

        // ===== Nhân viên =====
        var nvBinh = new NhanVien { MaTaiKhoan = tk[0].MaTaiKhoan, HoTen = tk[0].HoTen, SoDienThoai = SoDienThoaiLienLac, Email = tk[0].Email, ChucVu = "Quản lý cửa hàng" };
        var nvBang = new NhanVien { MaTaiKhoan = tk[1].MaTaiKhoan, HoTen = tk[1].HoTen, SoDienThoai = SoDienThoaiLienLac, Email = tk[1].Email, ChucVu = "Nhân viên tiếp nhận" };
        var nvGiang = new NhanVien { MaTaiKhoan = tk[2].MaTaiKhoan, HoTen = tk[2].HoTen, SoDienThoai = SoDienThoaiLienLac, Email = tk[2].Email, ChucVu = "Nhân viên tiếp nhận" };
        var nvHoangAnh = new NhanVien { MaTaiKhoan = tk[3].MaTaiKhoan, HoTen = tk[3].HoTen, SoDienThoai = SoDienThoaiLienLac, Email = tk[3].Email, ChucVu = "Nhân viên xử lý" };
        var nvKhanh = new NhanVien { MaTaiKhoan = tk[4].MaTaiKhoan, HoTen = tk[4].HoTen, SoDienThoai = SoDienThoaiLienLac, Email = tk[4].Email, ChucVu = "Nhân viên xử lý" };
        db.NhanViens.AddRange(nvBinh, nvBang, nvGiang, nvHoangAnh, nvKhanh);
        db.SaveChanges();
        var nvXuLy = new[] { nvHoangAnh, nvKhanh };

        // ===== Danh mục =====
        var loaiDefs = new[]
        {
            new { Ten = "Quần áo thường", MoTa = "Áo, quần vải thông dụng", Hd = true },
            new { Ten = "Áo vest", MoTa = "Vest nam, nữ và áo blazer", Hd = true },
            new { Ten = "Chăn ga", MoTa = "Chăn, ga, vỏ gối các loại", Hd = true },
            new { Ten = "Áo sơ mi", MoTa = "Sơ mi nam nữ", Hd = true },
            new { Ten = "Váy lụa", MoTa = "Váy, đầm chất liệu lụa, voan", Hd = true },
            new { Ten = "Giày", MoTa = "Giày da, giày thể thao", Hd = true },
            new { Ten = "Rèm cửa", MoTa = "Rèm cửa các loại", Hd = true },
            new { Ten = "Thú bông", MoTa = "Thú nhồi bông dưới 1m", Hd = false },
        };
        var loais = loaiDefs.Select(x => new LoaiDo { TenLoaiDo = x.Ten, MoTa = x.MoTa, TrangThai = x.Hd }).ToList();
        db.LoaiDos.AddRange(loais);

        var dvtDefs = new[] { ("Kg", "Tính theo khối lượng"), ("Chiếc", "Tính theo từng chiếc"), ("Bộ", "Bộ chăn ga, bộ rèm"), ("Đôi", "Giày, dép"), ("Cái", "Đồ lẻ"), ("Mét", "Tính theo chiều dài") };
        var dvts = dvtDefs.Select(x => new DonViTinh { TenDonViTinh = x.Item1, MoTa = x.Item2 }).ToList();
        db.DonViTinhs.AddRange(dvts);

        var dichVus = new List<DichVu>
        {
            new DichVu { TenDichVu = "Giặt thường",        MoTa = "Giặt máy, sấy khô, gấp gọn",       ThoiGianXuLyDuKien = 8 },
            new DichVu { TenDichVu = "Giặt ủi cao cấp",    MoTa = "Giặt nhẹ, là ủi và đóng gói",      ThoiGianXuLyDuKien = 12 },
            new DichVu { TenDichVu = "Giặt hấp",           MoTa = "Giặt hấp cho đồ cần giữ form",     ThoiGianXuLyDuKien = 24 },
            new DichVu { TenDichVu = "Giặt khô",           MoTa = "Giặt khô cho đồ cao cấp",          ThoiGianXuLyDuKien = 24 },
            new DichVu { TenDichVu = "Giặt chăn ga",       MoTa = "Giặt, sấy chăn ga gối",            ThoiGianXuLyDuKien = 36 },
            new DichVu { TenDichVu = "Giặt giày chuyên sâu", MoTa = "Vệ sinh giày chuyên sâu",        ThoiGianXuLyDuKien = 48, TrangThai = false },
        };
        db.DichVus.AddRange(dichVus);
        db.SaveChanges();

        LoaiDo Ld(string tenLoai) => loais.First(x => x.TenLoaiDo == tenLoai);
        DonViTinh Dv(string tenDvt) => dvts.First(x => x.TenDonViTinh == tenDvt);
        DichVu Dich(string tenDv) => dichVus.First(x => x.TenDichVu == tenDv);

        // ===== Bảng giá =====
        var d1 = now.Date.AddMonths(-6);
        var gDefs = new List<BangGiaDichVu>
        {
            new BangGiaDichVu { MaDichVu = Dich("Giặt thường").MaDichVu, MaLoaiDo = Ld("Quần áo thường").MaLoaiDo, MaDonViTinh = Dv("Kg").MaDonViTinh, DonGia = 25000, NgayApDung = d1.AddMonths(-6), NgayKetThuc = d1.AddDays(-1) },
            new BangGiaDichVu { MaDichVu = Dich("Giặt thường").MaDichVu, MaLoaiDo = Ld("Quần áo thường").MaLoaiDo, MaDonViTinh = Dv("Kg").MaDonViTinh, DonGia = 28000, NgayApDung = d1 },
            new BangGiaDichVu { MaDichVu = Dich("Giặt ủi cao cấp").MaDichVu, MaLoaiDo = Ld("Áo sơ mi").MaLoaiDo, MaDonViTinh = Dv("Chiếc").MaDonViTinh, DonGia = 45000, NgayApDung = d1 },
            new BangGiaDichVu { MaDichVu = Dich("Giặt ủi cao cấp").MaDichVu, MaLoaiDo = Ld("Quần áo thường").MaLoaiDo, MaDonViTinh = Dv("Kg").MaDonViTinh, DonGia = 45000, NgayApDung = d1 },
            new BangGiaDichVu { MaDichVu = Dich("Giặt ủi cao cấp").MaDichVu, MaLoaiDo = Ld("Váy lụa").MaLoaiDo, MaDonViTinh = Dv("Chiếc").MaDonViTinh, DonGia = 60000, NgayApDung = d1 },
            new BangGiaDichVu { MaDichVu = Dich("Giặt hấp").MaDichVu, MaLoaiDo = Ld("Áo vest").MaLoaiDo, MaDonViTinh = Dv("Chiếc").MaDonViTinh, DonGia = 120000, NgayApDung = d1.AddMonths(2) },
            new BangGiaDichVu { MaDichVu = Dich("Giặt hấp").MaDichVu, MaLoaiDo = Ld("Váy lụa").MaLoaiDo, MaDonViTinh = Dv("Chiếc").MaDonViTinh, DonGia = 85000, NgayApDung = d1.AddMonths(2) },
            new BangGiaDichVu { MaDichVu = Dich("Giặt hấp").MaDichVu, MaLoaiDo = Ld("Áo sơ mi").MaLoaiDo, MaDonViTinh = Dv("Chiếc").MaDonViTinh, DonGia = 85000, NgayApDung = d1.AddMonths(2) },
            new BangGiaDichVu { MaDichVu = Dich("Giặt khô").MaDichVu, MaLoaiDo = Ld("Áo vest").MaLoaiDo, MaDonViTinh = Dv("Chiếc").MaDonViTinh, DonGia = 120000, NgayApDung = d1.AddMonths(2) },
            new BangGiaDichVu { MaDichVu = Dich("Giặt khô").MaDichVu, MaLoaiDo = Ld("Váy lụa").MaLoaiDo, MaDonViTinh = Dv("Chiếc").MaDonViTinh, DonGia = 110000, NgayApDung = d1.AddMonths(2) },
            new BangGiaDichVu { MaDichVu = Dich("Giặt chăn ga").MaDichVu, MaLoaiDo = Ld("Chăn ga").MaLoaiDo, MaDonViTinh = Dv("Bộ").MaDonViTinh, DonGia = 160000, NgayApDung = d1.AddMonths(3) },
            new BangGiaDichVu { MaDichVu = Dich("Giặt chăn ga").MaDichVu, MaLoaiDo = Ld("Rèm cửa").MaLoaiDo, MaDonViTinh = Dv("Bộ").MaDonViTinh, DonGia = 140000, NgayApDung = d1.AddMonths(3) },
            new BangGiaDichVu { MaDichVu = Dich("Giặt giày chuyên sâu").MaDichVu, MaLoaiDo = Ld("Giày").MaLoaiDo, MaDonViTinh = Dv("Đôi").MaDonViTinh, DonGia = 120000, NgayApDung = d1.AddMonths(3) },
        };
        db.BangGias.AddRange(gDefs);
        db.SaveChanges();

        // ===== Khách hàng (38) =====
        var khs = new List<KhachHang>
        {
            new KhachHang { HoTen = "Nguyễn Minh Anh", SoDienThoai = "0983456821", Email = "minhanh@gmail.com", DiaChi = "18 Nguyễn Trãi, Thanh Xuân, Hà Nội" },
            new KhachHang { HoTen = "Trần Hoàng Nam", SoDienThoai = "0912802119", Email = "nam.tran@gmail.com", DiaChi = "25 Cầu Giấy, Hà Nội" },
            new KhachHang { HoTen = "Lê Thu Hương", SoDienThoai = "0904123665", Email = "huong.le@gmail.com", DiaChi = "102 Láng Hạ, Đống Đa, Hà Nội" },
            new KhachHang { HoTen = "Phạm Quang Huy", SoDienThoai = "0978444210", Email = "huypham@gmail.com", DiaChi = "7 Trần Duy Hưng, Cầu Giấy, Hà Nội" },
            new KhachHang { HoTen = "Đỗ Ngọc Mai", SoDienThoai = "0936781002", Email = "ngocmai@gmail.com", DiaChi = "56 Giải Phóng, Hai Bà Trưng, Hà Nội" },
            new KhachHang { HoTen = "Vũ Đức Long", SoDienThoai = "0902117839", Email = "long.vu@gmail.com", DiaChi = "31 Kim Mã, Ba Đình, Hà Nội" },
        };
        var hos = new[] { "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Vũ", "Đặng", "Bùi", "Đỗ", "Hồ", "Ngô", "Dương" };
        var dems = new[] { "Văn", "Thị", "Hữu", "Quang", "Thu", "Ngọc", "Minh", "Hải", "Thanh", "Đức" };
        var tens = new[] { "An", "Bình", "Chi", "Dũng", "Hà", "Hạnh", "Khoa", "Lan", "Linh", "Phong", "Quân", "Sơn", "Thảo", "Trang", "Tú", "Việt", "Yến", "Tài", "Hiếu", "Nga" };
        var duong = new[] { "Nguyễn Trãi", "Cầu Giấy", "Láng Hạ", "Kim Mã", "Giải Phóng", "Trần Duy Hưng", "Xuân Thủy", "Tây Sơn", "Lê Văn Lương", "Nguyễn Chí Thanh" };
        var sdtDaCo = new HashSet<string>(khs.Select(k => k.SoDienThoai));
        while (khs.Count < 38)
        {
            var h = hos[rnd.Next(hos.Length)]; var dm = dems[rnd.Next(dems.Length)]; var tn2 = tens[rnd.Next(tens.Length)];
            var hoTen = h + " " + dm + " " + tn2;
            string sdt;
            do { sdt = "09" + rnd.Next(10000000, 99999999).ToString(); } while (!sdtDaCo.Add(sdt));
            var email = (UiHelper.BoDau(tn2) + UiHelper.BoDau(h).Substring(0, 1)).ToLower() + rnd.Next(10, 99) + "@gmail.com";
            khs.Add(new KhachHang
            {
                HoTen = hoTen, SoDienThoai = sdt, Email = email,
                DiaChi = rnd.Next(1, 200) + " " + duong[rnd.Next(duong.Length)] + ", Hà Nội"
            });
        }
        khs[36].TrangThai = false; khs[37].TrangThai = false;   // 2 khách ngừng hoạt động
        db.KhachHangs.AddRange(khs);
        db.SaveChanges();
        var khHoatDong = khs.Where(k => k.TrangThai).ToList();

        // ===== Đơn giặt (60) =====
        var giaDangDung = gDefs.Where(b => b.NgayKetThuc == null && b.TrangThai
                                           && dichVus.First(d => d.MaDichVu == b.MaDichVu).TrangThai).ToList();
        var nguoiTiepNhan = new[] { tk[1], tk[2], tk[0] };
        var pttt = new[] { "Tiền mặt", "Chuyển khoản", "Quét QR" };
        var tinhTrangs = new[] { "Bình thường", "Ổ nhẹ vùng cổ áo", "1 áo có vết cà phê", "Có vết dầu mỡ nhỏ", "Sờn nhẹ ở gấu", "Bình thường" };
        var ghiChus = new string?[] { null, null, "Gọi khách trước 30 phút khi giao.", "Khách cần gấp trong ngày.", null, "Giao tận nơi sau 17h." };
        var dons = new List<DonGiat>();

        for (int i = 0; i < 60; i++)
        {
            int daysAgo = i < 8 ? 0 : rnd.Next(1, 30);
            DateTime tn = daysAgo == 0
                ? now.Date.AddMinutes(rnd.Next(0, Math.Max(1, (int)(now - now.Date).TotalMinutes)))
                : now.Date.AddDays(-daysAgo).AddHours(7 + rnd.Next(0, 11)).AddMinutes(rnd.Next(0, 60));

            // chọn trạng thái theo "tuổi" của đơn
            double r = rnd.NextDouble();
            string status;
            if (daysAgo <= 1) status = r < 0.15 ? TrangThaiDon.ChoGiaoTra : r < 0.50 ? TrangThaiDon.DangXuLy : r < 0.75 ? TrangThaiDon.DaXacNhan : TrangThaiDon.MoiTiepNhan;
            else if (daysAgo <= 4) status = r < 0.55 ? TrangThaiDon.HoanThanh : r < 0.75 ? TrangThaiDon.ChoGiaoTra : r < 0.90 ? TrangThaiDon.DangXuLy : TrangThaiDon.DaXacNhan;
            else status = r < 0.88 ? TrangThaiDon.HoanThanh : r < 0.92 ? TrangThaiDon.DaHuy : TrangThaiDon.ChoGiaoTra;

            var kh = khHoatDong[rnd.Next(khHoatDong.Count)];
            var nguoi = nguoiTiepNhan[rnd.Next(nguoiTiepNhan.Length)];

            // chọn 1-3 dòng bảng giá khác nhau
            int soDong = rnd.Next(1, 4);
            var chon = giaDangDung.OrderBy(x => rnd.Next()).Take(soDong).ToList();
            int maxGio = chon.Max(c => dichVus.First(d => d.MaDichVu == c.MaDichVu).ThoiGianXuLyDuKien);

            var don = new DonGiat
            {
                MaKhachHang = kh.MaKhachHang,
                NgayTiepNhan = tn,
                NgayHenTra = tn.AddHours(maxGio + 4),
                MaNguoiTiepNhan = nguoi.MaTaiKhoan,
                TrangThai = status,
                GhiChu = ghiChus[rnd.Next(ghiChus.Length)]
            };
            decimal tong = 0;
            foreach (var g in chon)
            {
                bool laKg = dvts.First(x => x.MaDonViTinh == g.MaDonViTinh).TenDonViTinh == "Kg";
                decimal sl = laKg ? rnd.Next(1, 6) : rnd.Next(1, 4);
                var thanhTien = sl * g.DonGia;
                tong += thanhTien;
                don.ChiTiets.Add(new ChiTietDonGiat
                {
                    MaDichVu = g.MaDichVu, MaLoaiDo = g.MaLoaiDo, MaDonViTinh = g.MaDonViTinh,
                    SoLuongKhoiLuong = sl, DonGiaApDung = g.DonGia, ThanhTien = thanhTien,
                    TinhTrangLucNhan = tinhTrangs[rnd.Next(tinhTrangs.Length)]
                });
            }
            don.TongTienDuKien = tong;

            void Log(string nd, DateTime tg, string? ng) =>
                don.LichSus.Add(new LichSuXuLy { NoiDung = nd, ThoiGian = Cap(tg), NguoiThucHien = ng });

            Log("Tiếp nhận đơn giặt", tn, nguoi.HoTen);
            if (status != TrangThaiDon.MoiTiepNhan) Log("Đơn đã được xác nhận", tn.AddMinutes(8), nguoi.HoTen);
            if (status == TrangThaiDon.DaHuy) Log("Đơn đã bị hủy theo yêu cầu khách hàng", tn.AddHours(3), nguoi.HoTen);

            // phân công + công đoạn
            bool coPhanCong = status == TrangThaiDon.DangXuLy || status == TrangThaiDon.ChoGiaoTra
                              || status == TrangThaiDon.HoanThanh || (status == TrangThaiDon.DaXacNhan && rnd.NextDouble() < 0.5);
            if (coPhanCong)
            {
                var nvx = nvXuLy[rnd.Next(nvXuLy.Length)];
                int xong = (status == TrangThaiDon.ChoGiaoTra || status == TrangThaiDon.HoanThanh) ? 6
                         : status == TrangThaiDon.DangXuLy ? rnd.Next(0, 5) : 0;
                string ttPc = xong == 6 ? QuyTrinh.HoanThanh : (status == TrangThaiDon.DangXuLy ? QuyTrinh.DangThucHien : QuyTrinh.ChuaBatDau);
                don.PhanCongs.Add(new PhanCongXuLy
                {
                    MaNhanVien = nvx.MaNhanVien, NgayPhanCong = Cap(tn.AddMinutes(25)), TrangThai = ttPc,
                    NoiDungCongViec = "Xử lý đơn giặt theo quy trình"
                });
                Log("Đã phân công xử lý cho " + nvx.HoTen, tn.AddMinutes(25), tk[0].HoTen);
                for (int s = 0; s < QuyTrinh.CongDoans.Length; s++)
                {
                    string st = s < xong ? QuyTrinh.HoanThanh
                              : (s == xong && status == TrangThaiDon.DangXuLy) ? QuyTrinh.DangThucHien
                              : QuyTrinh.ChuaBatDau;
                    var cd = new CongDoanXuLy
                    {
                        TenCongDoan = QuyTrinh.CongDoans[s], ThuTu = s + 1, MaNhanVien = nvx.MaNhanVien, TrangThai = st
                    };
                    if (st != QuyTrinh.ChuaBatDau) cd.ThoiGianBatDau = Cap(tn.AddMinutes(40 + s * 70));
                    if (st == QuyTrinh.HoanThanh) cd.ThoiGianKetThuc = Cap(tn.AddMinutes(95 + s * 70));
                    don.CongDoans.Add(cd);
                    if (st == QuyTrinh.DangThucHien)
                        Log("Đang thực hiện công đoạn " + QuyTrinh.CongDoans[s].ToLower(), tn.AddMinutes(40 + s * 70), nvx.HoTen);
                }
                if (xong == 6) Log("Hoàn tất xử lý, đơn chờ giao trả", tn.AddMinutes(95 + 5 * 70), nvx.HoTen);
            }

            // thanh toán
            void Tra(decimal soTien, DateTime tg) =>
                don.ThanhToans.Add(new ThanhToan { SoTien = soTien, NgayThanhToan = Cap(tg), PhuongThuc = pttt[rnd.Next(pttt.Length)], NguoiThucHien = nguoi.HoTen });
            decimal coc = Math.Round(tong * 0.5m / 1000m) * 1000m;
            if (status == TrangThaiDon.HoanThanh)
            {
                if (rnd.NextDouble() < 0.3 && coc > 0 && coc < tong) { Tra(coc, tn.AddMinutes(10)); Tra(tong - coc, don.NgayHenTra.AddHours(-2)); }
                else Tra(tong, don.NgayHenTra.AddHours(-2));
            }
            else if (status == TrangThaiDon.ChoGiaoTra)
            {
                double p = rnd.NextDouble();
                if (p < 0.55) Tra(tong, tn.AddHours(6)); else if (p < 0.8 && coc < tong) Tra(coc, tn.AddMinutes(10));
            }
            else if (status == TrangThaiDon.DangXuLy && rnd.NextDouble() < 0.35 && coc < tong) Tra(coc, tn.AddMinutes(10));
            else if ((status == TrangThaiDon.DaXacNhan || status == TrangThaiDon.MoiTiepNhan) && rnd.NextDouble() < 0.1 && coc < tong) Tra(coc, tn.AddMinutes(10));

            // giao trả
            if (status == TrangThaiDon.HoanThanh)
            {
                var ngayGiao = rnd.NextDouble() < 0.9 ? don.NgayHenTra.AddHours(-rnd.Next(1, 3)) : don.NgayHenTra.AddHours(rnd.Next(1, 12));
                ngayGiao = Cap(ngayGiao);
                don.GiaoTra = new GiaoTra
                {
                    NgayGiaoTra = ngayGiao, NguoiGiao = nguoi.HoTen, NguoiNhan = kh.HoTen,
                    HinhThucNhan = rnd.NextDouble() < 0.7 ? "Nhận tại cửa hàng" : "Giao tận nơi"
                };
                Log("Đã giao trả đồ cho khách hàng", ngayGiao, nguoi.HoTen);
            }
            dons.Add(don);
        }

        // ===== Khiếu nại =====
        var knDefs = new[]
        {
            ("Áo sơ mi còn vết ố ở cổ", "Cao", TrangThaiDon.DangXuLy),
            ("Giao đồ muộn hơn lịch hẹn", "Trung bình", "Mới tiếp nhận"),
            ("Nhầm túi đựng đồ", "Thấp", "Đã giải quyết"),
            ("Áo vest bị co rút nhẹ", "Cao", "Đang xử lý"),
            ("Mất một chiếc cúc áo", "Trung bình", "Đã giải quyết"),
            ("Giày chưa sạch hẳn phần đế", "Thấp", "Đã giải quyết"),
            ("Chăn còn mùi ẩm", "Trung bình", "Đang xử lý"),
            ("Thái độ phục vụ chưa nhiệt tình", "Thấp", "Mới tiếp nhận"),
            ("Phai màu ở tay áo", "Trung bình", "Đã giải quyết"),
            ("Thiếu một chiếc tất trong túi đồ", "Thấp", "Đã giải quyết"),
        };
        var nguoiPT = new[] { tk[0].HoTen, tk[1].HoTen, tk[2].HoTen };
        var donKN = dons.Where(d => d.TrangThai != TrangThaiDon.DaHuy && d.TrangThai != TrangThaiDon.MoiTiepNhan).OrderBy(d => rnd.Next()).Take(knDefs.Length).ToList();
        for (int i = 0; i < donKN.Count; i++)
        {
            var kn = knDefs[i];
            string tt = kn.Item3 == TrangThaiDon.DangXuLy ? "Đang xử lý" : kn.Item3;
            var d = donKN[i];
            donKN[i].KhieuNais.Add(new KhieuNai
            {
                NoiDung = kn.Item1, MucDo = kn.Item2, TrangThai = tt,
                NgayTao = Cap(d.NgayTiepNhan.AddHours(rnd.Next(4, 48))),
                NguoiPhuTrach = nguoiPT[rnd.Next(nguoiPT.Length)],
                HuongXuLy = tt == "Đã giải quyết" ? "Đã xin lỗi khách và xử lý lại miễn phí." : null,
                NgayHoanTat = tt == "Đã giải quyết" ? Cap(d.NgayTiepNhan.AddHours(60)) : (DateTime?)null
            });
        }

        db.DonGiats.AddRange(dons);
        db.SaveChanges();
    }
}
