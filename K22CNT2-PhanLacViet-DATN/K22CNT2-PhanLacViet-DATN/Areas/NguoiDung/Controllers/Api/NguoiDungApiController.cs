using K22CNT2_PhanLacViet_DATN.Areas.NguoiDung.Models;
using K22CNT2_PhanLacViet_DATN.Models;
using K22CNT2_PhanLacViet_DATN.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace K22CNT2_PhanLacViet_DATN.Areas.NguoiDung.Controllers.Api
{
    [Route("api/nguoidung")]
    [ApiController]
    public class NguoiDungApiController : ControllerBase
    {
        private readonly WebTruyenChuContext _context;

        public NguoiDungApiController(WebTruyenChuContext context)
        {
            _context = context;
        }

        [HttpGet("get-profile/{taiKhoan}")]
        public async Task<IActionResult> GetProfile(string taiKhoan)
        {
            if (string.IsNullOrEmpty(taiKhoan)) return BadRequest("Tài khoản không hợp lệ");

            var user = await _context.ThanhViens.FirstOrDefaultAsync(u => u.TaiKhoan == taiKhoan);
            if (user == null) return NotFound("Không tìm thấy người dùng");

            // --- 1. Lấy thống kê ---
            var countLichSu = await _context.LichSuDocs.CountAsync(x => x.TaiKhoan == taiKhoan);
            var countBinhLuan = await _context.BinhLuans.CountAsync(x => x.TaiKhoan == taiKhoan);
            var countDanhGia = await _context.DanhGia.CountAsync(x => x.TaiKhoan == taiKhoan);

            var countChuong = await _context.LichSuDocs
                .Where(x => x.TaiKhoan == taiKhoan)
                .SumAsync(x => x.MaChuongTruyenNavigation != null ? x.MaChuongTruyenNavigation.ThuTuChuong : 0);

            // --- 2. Khởi tạo ViewModel ---
            var viewModel = new UserProfileViewModel();

            // Fill thông tin cá nhân
            viewModel.ThongTinNguoiDung = new ThanhVienDto
            {
                TaiKhoan = user.TaiKhoan,
                // Nếu chưa có cột HoTen trong DB thì tạm dùng TaiKhoan, nếu có thì sửa thành user.HoTen
                HoTen = user.TaiKhoan,
                Avatar = user.Avatar,
                NgayThamGia = user.NgayTao.HasValue ? user.NgayTao.Value.ToString("dd/MM/yyyy") : "N/A",
                SoTruyenDaDoc = countLichSu,
                SoChuongDaDoc = countChuong,
                SoBinhLuan = countBinhLuan,
                SoDanhGia = countDanhGia
            };

            // --- 3. Lấy Danh sách Theo Dõi (Tủ truyện) ---
            viewModel.DsTheoDoi = await _context.TheoDois
                .Include(t => t.MaTruyenNavigation)
                .Where(t => t.TaiKhoan == taiKhoan)
                .OrderByDescending(t => t.NgayTheoDoi)
                .Select(t => new TruyenProfileItem
                {
                    MaTruyen = t.MaTruyen,
                    TenTruyen = t.MaTruyenNavigation.TenTruyen,
                    // Giả lập ảnh bìa nếu null
                    AnhBia = "https://via.placeholder.com/200x300?text=" + (t.MaTruyenNavigation.TenTruyen.Substring(0, 1) ?? "T"),
                    LuotXem = t.MaTruyenNavigation.TongLuotXem ?? 0,
                    ThoiGian = t.NgayTheoDoi.HasValue ? t.NgayTheoDoi.Value.ToString("dd/MM/yyyy") : "",
                    CoChuongMoi = false, // Cần logic check chương mới sau
                    LoaiDanhSach = "TheoDoi"
                }).ToListAsync();

            // --- 4. Lấy Danh sách Lịch Sử ---
            viewModel.DsLichSu = await _context.LichSuDocs
                .Include(ls => ls.MaTruyenNavigation)
                .Include(ls => ls.MaChuongTruyenNavigation)
                .Where(ls => ls.TaiKhoan == taiKhoan)
                .OrderByDescending(ls => ls.NgayDoc)
                .Take(10) // Lấy 10 cái gần nhất
                .Select(ls => new TruyenProfileItem
                {
                    MaTruyen = ls.MaTruyen,
                    TenTruyen = ls.MaTruyenNavigation.TenTruyen,
                    AnhBia = "https://via.placeholder.com/200x300?text=History",
                    TienDo = ls.MaChuongTruyenNavigation != null ? ls.MaChuongTruyenNavigation.TieuDe : "Đang đọc",
                    ThoiGian = ls.NgayDoc.HasValue ? ls.NgayDoc.Value.ToString("HH:mm dd/MM") : "",
                    LoaiDanhSach = "LichSu"
                }).ToListAsync();

            // --- 5. Lấy Danh sách Đã Lưu (Bookmark) ---
            viewModel.DsDaLuu = await _context.LuuTruyens
                .Include(l => l.MaTruyenNavigation)
                .Where(l => l.TaiKhoan == taiKhoan)
                .OrderByDescending(l => l.NgayLuu)
                .Select(l => new TruyenProfileItem
                {
                    MaTruyen = l.MaTruyen,
                    TenTruyen = l.MaTruyenNavigation.TenTruyen,
                    AnhBia = "https://via.placeholder.com/200x300?text=Saved",
                    LuotXem = l.MaTruyenNavigation.TongLuotXem ?? 0,
                    ThoiGian = l.NgayLuu.HasValue ? l.NgayLuu.Value.ToString("dd/MM/yyyy") : "",
                    LoaiDanhSach = "DaLuu"
                }).ToListAsync();

            return Ok(viewModel);
        }
    }
}
