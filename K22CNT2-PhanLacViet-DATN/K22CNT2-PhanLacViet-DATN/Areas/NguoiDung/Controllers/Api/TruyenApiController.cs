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
                .Select(l => new LichSuDocDto
                {
                    MaTruyen = l.MaTruyen,
                    TenTruyen = l.MaTruyenNavigation.TenTruyen,
                    ThuTuChuong = l.MaChuongTruyenNavigation.ThuTuChuong
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

            // 3. Lấy bình luận thông qua các chương của truyện này
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

            // 4. Tính điểm đánh giá (Sử dụng DanhGia - EF Scaffold thành DanhGia)
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
                DaDanhGia = daDanhGia
            };

            return Ok(result);
        }
    }
}
