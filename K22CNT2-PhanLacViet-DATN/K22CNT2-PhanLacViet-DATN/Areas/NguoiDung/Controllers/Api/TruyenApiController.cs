using K22CNT2_PhanLacViet_DATN.Areas.NguoiDung.Models;
using K22CNT2_PhanLacViet_DATN.Models;
using K22CNT2_PhanLacViet_DATN.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace K22CNT2_PhanLacViet_DATN.Areas.NguoiDung.Controllers.Api
{
    [Route("api/Truyen")]
    [ApiController]
    public class TruyenApiController : Controller
    {
        private readonly WebTruyenChuContext _context;

        public TruyenApiController(WebTruyenChuContext context)
        {
            _context = context;
        }

        // API: GET /api/Truyen
        [HttpGet]
        public async Task<IActionResult> GetAllTruyen()
        {
            var dsTruyen = await _context.Truyens
                                .Where(t => t.IsDeleted == false)
                                .OrderByDescending(t => t.NgayDang)
                                .ToListAsync();
            return Ok(dsTruyen);
        }
        [HttpGet("LichSu/{taiKhoan}")]
        public async Task<IActionResult> GetLichSu(string taiKhoan)
        {
            var lichSu = await _context.LichSuDocs
                .Where(l => l.TaiKhoan == taiKhoan)
                .OrderByDescending(ls => ls.NgayDoc)
                .Select(l => new LichSuDocDto
                {
                    MaTruyen = l.MaTruyen,
                    TenTruyen = l.MaTruyenNavigation.TenTruyen,
                    AnhBia = l.MaTruyenNavigation.AnhBia,
                    ThuTuChuong = l.MaChuongTruyenNavigation != null ? l.MaChuongTruyenNavigation.ThuTuChuong : 0,
                    NgayDoc = l.NgayDoc
                })
                .ToListAsync();

            return Ok(lichSu);
        }

        [HttpGet("DaLuu/{taiKhoan}")]
        public async Task<IActionResult> GetDaLuu(string taiKhoan)
        {
            var daLuu = await _context.LuuTruyens
                .Where(l => l.TaiKhoan == taiKhoan)
                .Select(l => l.MaTruyenNavigation)
                .ToListAsync();
            return Ok(daLuu);
        }
        // GET: api/Truyen/ChiTiet/5?taiKhoan=admin
        [HttpGet("ChiTiet/{id}")]
        public async Task<IActionResult> GetChiTietTruyen(int id, [FromQuery] string? taiKhoan = null)
        {
            // 1. Lấy thông tin truyện và Thể loại (Dùng Include để lấy bảng trung gian TruyenTheLoai)
            var truyenDb = await _context.Truyens
                .Include(t => t.MaTheLoais)
                .FirstOrDefaultAsync(t => t.MaTruyen == id);

            if (truyenDb == null) return NotFound();

            var truyenDto = new TruyenDto
            {
                MaTruyen = truyenDb.MaTruyen,
                TenTruyen = truyenDb.TenTruyen,
                TacGia = truyenDb.TacGia,
                MoTa = truyenDb.MoTa,
                LoaiTruyen = truyenDb.LoaiTruyen,
                TongLuotXem = truyenDb.TongLuotXem ?? 0,
                NgayDang = truyenDb.NgayDang,
                AnhBia = truyenDb.AnhBia,
                TrangThai = truyenDb.TrangThai,
                // Map thể loại từ danh sách liên kết
                TenTheLoai = truyenDb.MaTheLoais != null
                             ? string.Join(", ", truyenDb.MaTheLoais.Select(tl => tl.TenTheLoai))
                             : "Khác"
            };

            // 2. Lấy danh sách chương
            var dsChuong = await _context.ChuongTruyens
                .Where(x => x.MaTruyen == id)
                .OrderBy(x => x.ThuTuChuong)
                .Select(x => new ChuongTruyenDto
                {
                    MaChuongTruyen = x.MaChuongTruyen,
                    MaTruyen = x.MaTruyen,
                    ThuTuChuong = x.ThuTuChuong,
                    TieuDe = x.TieuDe,
                    NgayDang = x.NgayDang
                }).ToListAsync();

            // 3. Khởi tạo các trạng thái người dùng
            int? thuTuDaDoc = 0;
            bool daTheoDoi = false;
            bool daDanhGia = false;

            // 4. Kiểm tra logic nếu có người dùng đăng nhập
            if (!string.IsNullOrWhiteSpace(taiKhoan))
            {
                string tkNorm = taiKhoan.Trim();
                var lichSu = await _context.LichSuDocs
                    .Where(ls => ls.TaiKhoan.Trim() == tkNorm && ls.MaTruyen == id)
                    .Include(ls => ls.MaChuongTruyenNavigation)
                    .FirstOrDefaultAsync();

                thuTuDaDoc = lichSu?.MaChuongTruyenNavigation?.ThuTuChuong ?? 0;
                daTheoDoi = await _context.TheoDois
                    .AnyAsync(x => x.MaTruyen == id && x.TaiKhoan.Trim() == tkNorm);
                daDanhGia = await _context.DanhGia
                    .AnyAsync(x => x.MaTruyen == id && x.TaiKhoan.Trim() == tkNorm);
            }
            var maChuongIds = dsChuong.Select(c => c.MaChuongTruyen).ToList();
            var dsBinhLuan = await _context.BinhLuans
                .Where(bl => bl.MaChuongTruyen != null && maChuongIds.Contains(bl.MaChuongTruyen.Value))
                .Select(bl => new BinhLuanDto
                {
                    MaBinhLuan = bl.MaBinhLuan,
                    NoiDung = bl.NoiDung,
                    TaiKhoan = bl.TaiKhoan,
                    Avatar = bl.TaiKhoanNavigation != null ? (bl.TaiKhoanNavigation.Avatar ?? "/images/default-avatar.png") : "/images/default-avatar.png",
                    NgayGui = bl.NgayGui,
                    TenChuong = bl.MaChuongTruyenNavigation != null ? bl.MaChuongTruyenNavigation.TieuDe : "Chương không xác định",
                    RepBinhLuans = bl.RepBinhLuans.Select(r => new RepBinhLuanDto
                    {
                        MaRep = r.MaRep,
                        TaiKhoan = r.TaiKhoan,
                        Avatar = bl.TaiKhoanNavigation != null ? (bl.TaiKhoanNavigation.Avatar ?? "/images/default-avatar.png") : "/images/default-avatar.png",
                        NoiDung = r.NoiDung,
                        NgayGui = r.NgayGui
                    }).ToList()
                }).OrderByDescending(x => x.NgayGui).ToListAsync();
            var queryDanhGia = _context.DanhGia.Where(dg => dg.MaTruyen == id);
            double diemTB = 0;
            if (await queryDanhGia.AnyAsync())
            {
                diemTB = await queryDanhGia.AverageAsync(dg => (double)(dg.Diem ?? 0));
            }
            var result = new TrangTruyenViewModel
            {
                ThongTinTruyen = truyenDto,
                DanhSachChuong = dsChuong,
                DanhSachBinhLuan = dsBinhLuan,
                DiemDanhGiaTrungBinh = Math.Round(diemTB, 1),
                DaTheoDoi = daTheoDoi,
                DaDanhGia = daDanhGia,
                ThuTuChuongDaDoc = thuTuDaDoc
            };

            return Ok(result);
        }
        // API: POST /api/Truyen/TheoDoi
        [HttpPost("TheoDoi")]
        public async Task<IActionResult> ToggleTheoDoi([FromBody] TuongTacDto input)
        {
            if (string.IsNullOrEmpty(input.TaiKhoan)) return BadRequest("Chưa đăng nhập");

            var theoDoi = await _context.TheoDois
                .FirstOrDefaultAsync(x => x.TaiKhoan == input.TaiKhoan && x.MaTruyen == input.MaTruyen);

            bool isFollowed;

            if (theoDoi != null)
            {
                _context.TheoDois.Remove(theoDoi);
                isFollowed = false;
            }
            else
            {
                var newTheoDoi = new TheoDoi
                {
                    TaiKhoan = input.TaiKhoan,
                    MaTruyen = input.MaTruyen,
                    NgayTheoDoi = DateTime.Now
                };
                _context.TheoDois.Add(newTheoDoi);
                isFollowed = true;

                //GỬI THÔNG BÁO CHO TÁC GIẢ
                var truyen = await _context.Truyens.FindAsync(input.MaTruyen);
                if (truyen != null && truyen.NguoiDang != input.TaiKhoan) 
                {
                    var thongBao = new ThongBao
                    {
                        TaiKhoan = truyen.NguoiDang, 
                        NoiDung = $"{input.TaiKhoan} đã theo dõi truyện '{truyen.TenTruyen}' của bạn.",
                        DaDoc = false,
                        NgayGui = DateTime.Now
                    };
                    _context.ThongBaos.Add(thongBao);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { daTheoDoi = isFollowed });
        }

        // API: POST /api/Truyen/DanhGia
        [HttpPost("DanhGia")]
        public async Task<IActionResult> PostDanhGia([FromBody] DanhGiaInputDto input)
        {
            if (string.IsNullOrEmpty(input.TaiKhoan)) return BadRequest("Chưa đăng nhập");
            var danhGiaCu = await _context.DanhGia
                .FirstOrDefaultAsync(x => x.TaiKhoan == input.TaiKhoan && x.MaTruyen == input.MaTruyen);
            if (danhGiaCu != null)
            {
                danhGiaCu.Diem = input.Diem;
                danhGiaCu.NoiDung = input.NoiDung;
                danhGiaCu.NgayDanhGia = DateTime.Now;
            }
            else
            {
                var moi = new DanhGium 
                {
                    TaiKhoan = input.TaiKhoan,
                    MaTruyen = input.MaTruyen,
                    Diem = input.Diem,
                    NoiDung = input.NoiDung,
                    NgayDanhGia = DateTime.Now
                };
                _context.DanhGia.Add(moi);
                var truyen = await _context.Truyens.FindAsync(input.MaTruyen);
                if (truyen != null && truyen.NguoiDang != input.TaiKhoan)
                {
                    var thongBao = new ThongBao
                    {
                        TaiKhoan = truyen.NguoiDang,
                        NoiDung = $"{input.TaiKhoan} đã đánh giá {input.Diem} sao cho truyện '{truyen.TenTruyen}' của bạn.",
                        DaDoc = false,
                        NgayGui = DateTime.Now
                    };
                    _context.ThongBaos.Add(thongBao);
                }
            }
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Đánh giá thành công" });
        }
        // GET: api/Truyen/Chuong/5
        [HttpGet("Chuong/{id}/{taiKhoan}")]
        public async Task<IActionResult> GetChiTietChuong(int id, string? taiKhoan = null)
        {
            // 1. Lấy chương hiện tại
            var chuong = await _context.ChuongTruyens
                .FirstOrDefaultAsync(x => x.MaChuongTruyen == id);

            if (chuong == null) return NotFound();

            var chuongDto = new ChuongTruyenDto
            {
                MaChuongTruyen = chuong.MaChuongTruyen,
                MaTruyen = chuong.MaTruyen,
                ThuTuChuong = chuong.ThuTuChuong,
                TieuDe = chuong.TieuDe,
                NoiDung = chuong.NoiDung, // Nội dung truyện nằm ở đây
                NgayDang = chuong.NgayDang
            };

            //2. tăng lượt xem
            try
            {
                var truyen = await _context.Truyens.FindAsync(chuong.MaTruyen);
                if (truyen != null)
                {
                    truyen.TongLuotXem = (truyen.TongLuotXem ?? 0) + 1;
                }
                var homNay = DateTime.Today; 
                var thongKeNgay = await _context.LuotXemTruyens
                    .FirstOrDefaultAsync(lx => lx.MaTruyen == chuong.MaTruyen && lx.Ngay == homNay);

                if (thongKeNgay != null)
                {
                    thongKeNgay.SoLuotXem += 1;
                }
                else
                {
                    var moi = new LuotXemTruyen
                    {
                        MaTruyen = chuong.MaTruyen,
                        Ngay = homNay,
                        SoLuotXem = 1
                    };
                    _context.LuotXemTruyens.Add(moi);
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi cập nhật lượt xem: " + ex.Message);
            }

            // 3. LOGIC LƯU LỊCH SỬ ĐỌC
            if (!string.IsNullOrEmpty(taiKhoan) && taiKhoan.ToLower() != "guest")
            {
                try
                {
                    var lichSu = await _context.LichSuDocs
                        .FirstOrDefaultAsync(ls => ls.TaiKhoan == taiKhoan && ls.MaTruyen == chuong.MaTruyen);

                    if (lichSu != null)
                    {
                        lichSu.MaChuongTruyen = id;
                        lichSu.NgayDoc = DateTime.Now;
                        _context.LichSuDocs.Update(lichSu);
                    }
                    else
                    {
                        var newLichSu = new K22CNT2_PhanLacViet_DATN.Models.LichSuDoc
                        {
                            TaiKhoan = taiKhoan,
                            MaTruyen = chuong.MaTruyen,
                            MaChuongTruyen = id,
                            NgayDoc = DateTime.Now
                        };
                        _context.LichSuDocs.Add(newLichSu);
                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Lỗi lưu lịch sử: " + ex.Message);
                }
            }

            // 4. Tìm ID chương trước và chương sau dựa trên Thứ tự chương
            var chuongTruoc = await _context.ChuongTruyens
                .Where(x => x.MaTruyen == chuong.MaTruyen && x.ThuTuChuong < chuong.ThuTuChuong)
                .OrderByDescending(x => x.ThuTuChuong)
                .Select(x => x.MaChuongTruyen)
                .FirstOrDefaultAsync();

            var chuongSau = await _context.ChuongTruyens
                .Where(x => x.MaTruyen == chuong.MaTruyen && x.ThuTuChuong > chuong.ThuTuChuong)
                .OrderBy(x => x.ThuTuChuong)
                .Select(x => x.MaChuongTruyen)
                .FirstOrDefaultAsync();

            // 5. Lấy bình luận của riêng chương này
            var dsBinhLuan = await _context.BinhLuans
                .Where(bl => bl.MaChuongTruyen == id) // Chỉ lấy của chương này
                .Select(bl => new BinhLuanDto
                {
                    MaBinhLuan = bl.MaBinhLuan,
                    NoiDung = bl.NoiDung,
                    TaiKhoan = bl.TaiKhoan,
                    NgayGui = bl.NgayGui,
                    Avatar = bl.TaiKhoanNavigation != null ? (bl.TaiKhoanNavigation.Avatar ?? "/images/default-avatar.png") : "/images/default-avatar.png",
                    RepBinhLuans = bl.RepBinhLuans.Select(r => new RepBinhLuanDto
                    {
                        MaRep = r.MaRep,
                        TaiKhoan = r.TaiKhoan,
                        NoiDung = r.NoiDung,
                        Avatar = r.TaiKhoanNavigation != null ? (r.TaiKhoanNavigation.Avatar ?? "/images/default-avatar.png") : "/images/default-avatar.png",
                        NgayGui = r.NgayGui
                    }).ToList()
                })
                .OrderByDescending(x => x.NgayGui)
                .ToListAsync();

            var result = new ChiTietChuongViewModel
            {
                ChuongHienTai = chuongDto,
                MaChuongTruoc = chuongTruoc == 0 ? (int?)null : chuongTruoc,
                MaChuongSau = chuongSau == 0 ? (int?)null : chuongSau,
                MaTruyen = chuong.MaTruyen,
                DanhSachBinhLuan = dsBinhLuan
            };

            return Ok(result);
        }
        [HttpPost("LuuTruyen")]
        public async Task<IActionResult> LuuTruyen([FromBody] LuuTruyenInput input)
        {
            // 1. Kiểm tra đã lưu chưa
            var daTonTai = await _context.LuuTruyens
                .AnyAsync(x => x.TaiKhoan == input.TaiKhoan && x.MaTruyen == input.MaTruyen);

            if (daTonTai)
                return Ok(new { success = false, message = "Truyện đã có trong tủ." });

            // 2. Thêm mới
            var tuTruyen = new LuuTruyen
            {
                TaiKhoan = input.TaiKhoan,
                MaTruyen = input.MaTruyen,
                NgayLuu = DateTime.Now
            };

            _context.LuuTruyens.Add(tuTruyen);
            var thongTinTruyen = await _context.ChuongTruyens
                .Where(c => c.MaTruyen == input.MaTruyen)
                .Select(c => new { c.MaTruyenNavigation.NguoiDang, c.MaTruyenNavigation.TenTruyen, c.TieuDe })
                .FirstOrDefaultAsync();

            if (thongTinTruyen != null && thongTinTruyen.NguoiDang != input.TaiKhoan)
            {
                var thongBao = new ThongBao
                {
                    TaiKhoan = thongTinTruyen.NguoiDang,
                    NoiDung = $"{input.TaiKhoan} đã lưu truyện '{thongTinTruyen.TenTruyen}' của bạn.",
                    DaDoc = false,
                    NgayGui = DateTime.Now
                };
                _context.ThongBaos.Add(thongBao);
            }
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
        [HttpPost("BinhLuan/Them")]
        public async Task<IActionResult> ThemBinhLuan([FromBody] ThemBinhLuanInput input)
        {
            var bl = new BinhLuan
            {
                TaiKhoan = input.TaiKhoan,
                MaChuongTruyen = input.MaChuongId,
                NoiDung = input.NoiDung,
                NgayGui = DateTime.Now
            };
            _context.BinhLuans.Add(bl);
            var thongTinTruyen = await _context.ChuongTruyens
                .Where(c => c.MaChuongTruyen == input.MaChuongId)
                .Select(c => new { c.MaTruyenNavigation.NguoiDang, c.MaTruyenNavigation.TenTruyen, c.TieuDe })
                .FirstOrDefaultAsync();

            if (thongTinTruyen != null && thongTinTruyen.NguoiDang != input.TaiKhoan)
            {
                var thongBao = new ThongBao
                {
                    TaiKhoan = thongTinTruyen.NguoiDang,
                    NoiDung = $"{input.TaiKhoan} đã bình luận tại chương '{thongTinTruyen.TieuDe}' truyện '{thongTinTruyen.TenTruyen}' của bạn.",
                    DaDoc = false,
                    NgayGui = DateTime.Now
                };
                _context.ThongBaos.Add(thongBao);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("BinhLuan/TraLoi")]
        public async Task<IActionResult> ThemRepBinhLuan([FromBody] ThemBinhLuanInput input)
        {
            var bl = new RepBinhLuan
            {
                TaiKhoan = input.TaiKhoan,
                MaBinhLuan = input.MaBinhLuanGoc,
                NoiDung = input.NoiDung,
                NgayGui = DateTime.Now
            };
            _context.RepBinhLuans.Add(bl);
            try
            {
                var binhLuanGoc = await _context.BinhLuans
                    .FirstOrDefaultAsync(x => x.MaBinhLuan == input.MaBinhLuanGoc);
                if (binhLuanGoc != null && binhLuanGoc.TaiKhoan != input.TaiKhoan)
                {
                    var thongBao = new ThongBao
                    {
                        TaiKhoan = binhLuanGoc.TaiKhoan,
                        NoiDung = $"{input.TaiKhoan} đã rep bình luận của bạn.",
                        DaDoc = false,
                        NgayGui = DateTime.Now
                    };
                    _context.ThongBaos.Add(thongBao);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Lỗi gửi thông báo rep bình luận: " + ex.Message);
            }
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("TimKiem")]
        public async Task<IActionResult> TimKiem(
            string? keyword = null,
            string? tacGia = null,
            string? moTa = null,
            string? loaiTruyen = null, 
            string? luotXemRange = null,
            string? sort = "NgayDang_Desc",
            [FromQuery] List<int>? theLoais = null
        )
        {
            var query = _context.Truyens
                .Include(t => t.MaTheLoais)
                .Where(t => t.IsDeleted == false)
                .AsQueryable();

            // Lọc theo từ khóa (Tên truyện)
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.TenTruyen.Contains(keyword));
            }
            // Lọc theo tác giả
            if (!string.IsNullOrEmpty(tacGia))
            {
                query = query.Where(t => t.TacGia!.Contains(tacGia));
            }
            // Lọc theo mô tả
            if (!string.IsNullOrEmpty(moTa))
            {
                query = query.Where(t => t.MoTa!.Contains(moTa));
            }
            // Lọc theo Loại truyện
            if (!string.IsNullOrEmpty(loaiTruyen))
            {
                query = query.Where(t => t.LoaiTruyen == loaiTruyen);
            }

            // Lọc theo Khoảng lượt xem
            if (!string.IsNullOrEmpty(luotXemRange))
            {
                switch (luotXemRange)
                {
                    case "1": query = query.Where(t => t.TongLuotXem >= 0 && t.TongLuotXem <= 1000); break;
                    case "2": query = query.Where(t => t.TongLuotXem > 1000 && t.TongLuotXem <= 10000); break;
                    case "3": query = query.Where(t => t.TongLuotXem > 10000 && t.TongLuotXem <= 100000); break;
                    case "4": query = query.Where(t => t.TongLuotXem > 100000); break;
                }
            }
            // Lọc theo thể loại (Nếu có chọn)
            if (theLoais != null && theLoais.Any())
            {
                query = query.Where(t => t.MaTheLoais.Any(tl => theLoais.Contains(tl.MaTheLoai)));
            }

            // 5. Sắp xếp
            switch (sort)
            {
                case "NgayDang_Asc": query = query.OrderBy(t => t.NgayDang); break;
                case "LuotXem_Desc": query = query.OrderByDescending(t => t.TongLuotXem); break;
                case "LuotXem_Asc": query = query.OrderBy(t => t.TongLuotXem); break;
                default: query = query.OrderByDescending(t => t.NgayDang); break; // Mặc định
            }

            var dsTruyen = await query.Select(t => new TruyenDto
            {
                MaTruyen = t.MaTruyen,
                TenTruyen = t.TenTruyen,
                TacGia = t.TacGia,
                MoTa = t.MoTa, // Có thể cắt ngắn nếu cần
                TongLuotXem = t.TongLuotXem ?? 0,
                NgayDang = t.NgayDang,
                NgayCapNhat = t.NgayCapNhat ?? t.NgayDang,
                AnhBia = t.AnhBia,
                TenTheLoai = string.Join(", ", t.MaTheLoais.Select(tl => tl.TenTheLoai))
            }).ToListAsync();

            return Ok(dsTruyen);
        }
        [HttpGet("TheLoai")]
        public async Task<IActionResult> GetTheLoais()
        {
            var list = await _context.TheLoais
                .Select(tl => new { Id = tl.MaTheLoai, Ten = tl.TenTheLoai })
                .ToListAsync();
            return Ok(list);
        }
        [HttpGet("BangXepHang/{loai}")] // loai = ngay, tuan, thang
        public async Task<IActionResult> GetBangXepHang(string loai)
        {
            var query = _context.LuotXemTruyens.AsQueryable();
            var today = DateTime.Now.Date;
            switch (loai.ToLower())
            {
                case "ngay":
                    query = query.Where(x => x.Ngay == today);
                    break;
                case "tuan":
                    // Lấy 7 ngày gần nhất
                    var startWeek = today.AddDays(-7);
                    query = query.Where(x => x.Ngay >= startWeek);
                    break;
                case "thang":
                    // Lấy 30 ngày gần nhất
                    var startMonth = today.AddDays(-30);
                    query = query.Where(x => x.Ngay >= startMonth);
                    break;
                default:
                    return BadRequest("Loại xếp hạng không hợp lệ");
            }

            var result = await query
                .GroupBy(x => x.MaTruyen)
                .Select(g => new
                {
                    MaTruyen = g.Key,
                    TongLuotXem = g.Sum(x => x.SoLuotXem)
                })
                .OrderByDescending(x => x.TongLuotXem)
                .Take(10)
                .Join(_context.Truyens,
                    lx => lx.MaTruyen,
                    t => t.MaTruyen,
                    (lx, t) => new TruyenDto
                    {
                        MaTruyen = t.MaTruyen,
                        TenTruyen = t.TenTruyen,
                        AnhBia = t.AnhBia,
                        TacGia = t.TacGia,
                        LuotXem = lx.TongLuotXem ?? 0,
                        NgayCapNhat = t.NgayCapNhat ?? DateTime.Now
                    })
                .ToListAsync();

            return Ok(result);
        }
        
    }
}
