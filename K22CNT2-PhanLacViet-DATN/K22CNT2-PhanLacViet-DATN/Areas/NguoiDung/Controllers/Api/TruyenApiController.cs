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
    public class TruyenApiController : ControllerBase
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
        public async Task<IActionResult> GetChiTietTruyen(int id, string? taiKhoan = null)
        {
            // 1. Lấy thông tin truyện và Thể loại
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
                // Lấy danh sách tên thể loại nối với nhau
                TenTheLoai = string.Join(", ", truyenDb.MaTheLoais.Select(tl => tl.TenTheLoai))
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

            int? thuTuDaDoc = 0;
            if (!string.IsNullOrEmpty(taiKhoan))
            {
                thuTuDaDoc = await _context.LichSuDocs
                    .Where(ls => ls.TaiKhoan == taiKhoan && ls.MaTruyen == id)
                    .Select(ls => (int?)ls.MaChuongTruyenNavigation.ThuTuChuong)
                    .FirstOrDefaultAsync() ?? 0;
            }

            // 4. Lấy bình luận thông qua các chương của truyện này
            var maChuongIds = dsChuong.Select(c => c.MaChuongTruyen).ToList();
            var dsBinhLuan = await _context.BinhLuans
                .Where(bl => bl.MaChuongTruyen != null && maChuongIds.Contains(bl.MaChuongTruyen.Value))
                .Select(bl => new BinhLuanDto
                {
                    MaBinhLuan = bl.MaBinhLuan,
                    NoiDung = bl.NoiDung,
                    TaiKhoan = bl.TaiKhoan,
                    NgayGui = bl.NgayGui,
                    RepBinhLuans = bl.RepBinhLuans.Select(r => new RepBinhLuanDto
                    {
                        MaRep = r.MaRep,
                        TaiKhoan = r.TaiKhoan,
                        NoiDung = r.NoiDung,
                        NgayGui = r.NgayGui
                    }).ToList()
                }).OrderByDescending(x => x.NgayGui).ToListAsync();

            // 5. Tính điểm đánh giá (Sử dụng DanhGia - EF Scaffold thành DanhGia)
            // Lưu ý: Trong Model của bạn là truyenDb.DanhGia (ICollection<DanhGium>)
            var danhGias = await _context.DanhGia.Where(dg => dg.MaTruyen == id).ToListAsync();
            double diemTB = danhGias.Any() ? (double)danhGias.Average(dg => dg.Diem ?? 0) : 0;

            // 5. Kiểm tra trạng thái User
            bool daTheoDoi = false;
            bool daDanhGia = false;
            if (!string.IsNullOrEmpty(taiKhoan))
            {
                daTheoDoi = await _context.TheoDois.AnyAsync(x => x.MaTruyen == id && x.TaiKhoan == taiKhoan);
                daDanhGia = await _context.DanhGia.AnyAsync(x => x.MaTruyen == id && x.TaiKhoan == taiKhoan);
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

            // 2. LOGIC LƯU LỊCH SỬ ĐỌC
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
                    RepBinhLuans = bl.RepBinhLuans.Select(r => new RepBinhLuanDto
                    {
                        MaRep = r.MaRep,
                        TaiKhoan = r.TaiKhoan,
                        NoiDung = r.NoiDung,
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
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("TimKiem")]
        public async Task<IActionResult> TimKiem(
            string? keyword = null,
            string? tacGia = null,
            string? moTa = null,
            string? sort = "NgayDang_Desc",
            [FromQuery] List<int>? theLoais = null
        )
        {
            var query = _context.Truyens
                .Include(t => t.MaTheLoais)
                .Where(t => t.IsDeleted == false)
                .AsQueryable();

            // 1. Lọc theo từ khóa (Tên truyện)
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(t => t.TenTruyen.Contains(keyword));
            }
            // 2. Lọc theo tác giả
            if (!string.IsNullOrEmpty(tacGia))
            {
                query = query.Where(t => t.TacGia.Contains(tacGia));
            }
            // 3. Lọc theo mô tả
            if (!string.IsNullOrEmpty(moTa))
            {
                query = query.Where(t => t.MoTa.Contains(moTa));
            }
            // 4. Lọc theo thể loại (Nếu có chọn)
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
    }
}
