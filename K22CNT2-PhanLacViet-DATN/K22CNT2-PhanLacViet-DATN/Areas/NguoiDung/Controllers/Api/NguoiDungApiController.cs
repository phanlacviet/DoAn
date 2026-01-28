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
                    AnhBia = t.MaTruyenNavigation.AnhBia ?? "/images/AnhBia/default.jpg",
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
                    AnhBia = ls.MaTruyenNavigation.AnhBia ?? "/images/AnhBia/default.jpg",
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
                    AnhBia = l.MaTruyenNavigation.AnhBia ?? "/images/AnhBia/default.jpg",
                    LuotXem = l.MaTruyenNavigation.TongLuotXem ?? 0,
                    ThoiGian = l.NgayLuu.HasValue ? l.NgayLuu.Value.ToString("dd/MM/yyyy") : "",
                    LoaiDanhSach = "DaLuu"
                }).ToListAsync();

            return Ok(viewModel);
        }
        [HttpGet("dashboard/{taiKhoan}")]
        public async Task<IActionResult> GetTacGiaDashboard(string taiKhoan)
        {
            if (string.IsNullOrEmpty(taiKhoan)) return BadRequest("Tài khoản không hợp lệ");

            // 1. Lấy danh sách truyện
            var listTruyen = await _context.Truyens
                .Where(t => t.NguoiDang == taiKhoan && t.IsDeleted == false)
                .Select(t => new TruyenDashboardDto
                {
                    MaTruyen = t.MaTruyen,
                    TenTruyen = t.TenTruyen,
                    AnhBia = t.AnhBia,
                    NgayCapNhat = t.NgayCapNhat,
                    SoChuong = _context.ChuongTruyens.Count(c => c.MaTruyen == t.MaTruyen),
                    LuotTheoDoi = _context.TheoDois.Count(td => td.MaTruyen == t.MaTruyen),
                    LuotLuu = _context.LuuTruyens.Count(lt => lt.MaTruyen == t.MaTruyen),
                    LuotBinhLuan = _context.BinhLuans
                        .Count(bl => bl.MaChuongTruyenNavigation!.MaTruyen == t.MaTruyen),
                    DiemDanhGia = _context.DanhGia.Where(dg => dg.MaTruyen == t.MaTruyen).Average(dg => (double?)dg.Diem) ?? 0,
                    LuotDanhGia = _context.DanhGia.Count(dg => dg.MaTruyen == t.MaTruyen)
                })
                .OrderByDescending(t => t.NgayCapNhat)
                .ToListAsync();
            var stats = new TacGiaStatsDto
            {
                TongLuotXem = await _context.Truyens.Where(t => t.NguoiDang == taiKhoan).SumAsync(t => (long?)t.TongLuotXem) ?? 0,
                TongNguoiTheoDoi = listTruyen.Sum(t => t.LuotTheoDoi),
                TongLuotLuu = listTruyen.Sum(t => t.LuotLuu),
                DiemDanhGiaTrungBinh = listTruyen.Any() ? listTruyen.Average(t => t.DiemDanhGia) : 0
            };
            var viewModel = new TacGiaViewModels
            {
                ThongKeChung = stats,
                DanhSachTruyen = listTruyen
            };

            return Ok(viewModel);
        }
        [HttpGet("GetChapters/{maTruyen}")]
        public IActionResult GetChapters(int maTruyen)
        {
            var chuongs = _context.ChuongTruyens
                .Where(c => c.MaTruyen == maTruyen)
                .OrderByDescending(c => c.ThuTuChuong) // Mới nhất lên đầu
                .Select(c => new
                {
                    c.MaChuongTruyen,
                    c.ThuTuChuong,
                    c.TieuDe,
                    NgayDang = c.NgayDang.HasValue ? c.NgayDang.Value.ToString("dd/MM/yyyy") : ""
                })
                .ToList();
            return Ok(chuongs);
        }
        [HttpGet("GetChartStats/{maTruyen}")]
        public IActionResult GetChartStats(int maTruyen)
        {
            var bayNgayTruoc = DateTime.Today.AddDays(-6);
            var homNay = DateTime.Today;
            var queryData = _context.LuotXemTruyens
                .Where(lx => lx.MaTruyen == maTruyen && lx.Ngay >= bayNgayTruoc)
                .ToList();
            var result = new List<ChartDataDto>();

            for (int i = 6; i >= 0; i--)
            {
                var currentDay = DateTime.Today.AddDays(-i);
                var dataInDay = queryData.FirstOrDefault(x => x.Ngay.HasValue && x.Ngay.Value.Date == currentDay.Date);
                result.Add(new ChartDataDto
                {
                    Label = currentDay.ToString("dd/MM"),
                    Value = dataInDay?.SoLuotXem ?? 0
                });
            }
            return Ok(result);
        }

        // API Xóa mềm truyện
        [HttpPost("DeleteStory/{maTruyen}")]
        public IActionResult DeleteStory(int maTruyen)
        {
            var truyen = _context.Truyens.Find(maTruyen);
            if (truyen == null) return NotFound();
            truyen.IsDeleted = true; // Xóa mềm
            _context.SaveChanges();
            return Ok(new { success = true, message = "Đã xóa truyện thành công" });
        }
        [HttpGet("GetNotifications/{taiKhoan}")]
        public async Task<IActionResult> GetNotifications(string taiKhoan)
        {
            if (string.IsNullOrEmpty(taiKhoan)) return BadRequest("Tài khoản không hợp lệ");

            try
            {
                var thongBaos = await _context.ThongBaos
                    .Where(tb => tb.TaiKhoan == taiKhoan)
                    .OrderByDescending(tb => tb.NgayGui)
                    .Select(tb => new
                    {
                        tb.MaThongBao,
                        TieuDe = tb.NoiDung!.Length > 50 ? tb.NoiDung.Substring(0, 50) + "..." : tb.NoiDung,
                        tb.NoiDung,
                        NgayGui = tb.NgayGui.HasValue ? tb.NgayGui.Value.ToString("dd/MM/yyyy HH:mm") : "",
                        DaDoc = tb.DaDoc 
                    })
                    .ToListAsync();

                return Ok(thongBaos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi truy vấn: " + ex.Message);
            }
        }
        [HttpPost("MarkAsRead/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var thongBao = await _context.ThongBaos.FindAsync(id);
                if (thongBao == null) return NotFound();
                thongBao.DaDoc = true; 
                _context.ThongBaos.Update(thongBao);
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi: " + ex.Message);
            }
        }
        [HttpPost("MarkAllRead/{taiKhoan}")]
        public async Task<IActionResult> MarkAllRead(string taiKhoan)
        {
            var list = await _context.ThongBaos.Where(x => x.TaiKhoan == taiKhoan && (x.DaDoc == false)).ToListAsync();
            foreach (var item in list) { item.DaDoc = true; }
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
        [HttpDelete("DeleteReadNotifications/{taiKhoan}")]
        public async Task<IActionResult> DeleteReadNotifications(string taiKhoan)
        {
            if (string.IsNullOrEmpty(taiKhoan)) return BadRequest();

            try
            {
                var readNotifications = await _context.ThongBaos
                    .Where(x => x.TaiKhoan == taiKhoan && x.DaDoc == true)
                    .ToListAsync();

                if (readNotifications.Any())
                {
                    _context.ThongBaos.RemoveRange(readNotifications);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { success = true, count = readNotifications.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi xóa: " + ex.Message);
            }
        }
        [HttpGet("GetAvatar/{taiKhoan}")]
        public async Task<IActionResult> GetAvatar(string taiKhoan)
        {
            var user = await _context.ThanhViens
                .Where(x => x.TaiKhoan == taiKhoan)
                .Select(x => new { x.Avatar })
                .FirstOrDefaultAsync();

            if (user == null || string.IsNullOrEmpty(user.Avatar))
            {
                return Ok(new { avatar = "/NguoiDung/images/Avatar/default.jpg" });
            }

            return Ok(new { avatar = user.Avatar });
        }
        [HttpPost("dang-truyen")]
        public async Task<IActionResult> PostDangTruyen([FromForm] DangTruyenDto input)
        {
            var taiKhoan = HttpContext.Session.GetString("USER_LOGIN");
            if (string.IsNullOrEmpty(taiKhoan)) return Unauthorized();

            try
            {
                string fileNameToSave = "default.jpg";
                if (input.FileAnhBia != null)
                {
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "AnhBia");
                    string originalFileName = Path.GetFileNameWithoutExtension(input.FileAnhBia.FileName);
                    string extension = Path.GetExtension(input.FileAnhBia.FileName);
                    fileNameToSave = originalFileName + extension;
                    string fullPath = Path.Combine(folderPath, fileNameToSave);

                    int count = 1;
                    while (System.IO.File.Exists(fullPath))
                    {
                        fileNameToSave = $"{originalFileName}({count}){extension}";
                        fullPath = Path.Combine(folderPath, fileNameToSave);
                        count++;
                    }

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await input.FileAnhBia.CopyToAsync(stream);
                    }
                }
                var truyen = new Truyen
                {
                    TenTruyen = input.TenTruyen,
                    MoTa = input.MoTa,
                    TacGia = input.TacGia,
                    LoaiTruyen = input.LoaiTruyen,
                    NguoiDang = taiKhoan,
                    AnhBia = "/images/AnhBia/" + fileNameToSave,
                    TrangThai = "Đang ra",
                    NgayDang = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    IsDeleted = false
                };
                if (input.SelectedTheLoais != null && input.SelectedTheLoais.Any())
                {
                    var listTheLoaiSelected = await _context.TheLoais
                        .Where(tl => input.SelectedTheLoais.Contains(tl.MaTheLoai))
                        .ToListAsync();
                    truyen.MaTheLoais = listTheLoaiSelected;
                }

                _context.Truyens.Add(truyen);
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // --- SỬA TRUYỆN ---
        [HttpPut("sua-truyen")]
        public async Task<IActionResult> PutSuaTruyen([FromForm] SuaTruyenDto input)
        {
            var truyen = await _context.Truyens.Include(t => t.MaTheLoais).FirstOrDefaultAsync(t => t.MaTruyen == input.MaTruyen);
            if (truyen == null) return NotFound(new { message = "Không tìm thấy truyện" });
            truyen.TenTruyen = input.TenTruyen;
            truyen.MoTa = input.MoTa;
            truyen.TacGia = input.TacGia;
            truyen.LoaiTruyen = input.LoaiTruyen;
            truyen.NgayCapNhat = DateTime.Now;
            string fileNameToSave = "default.jpg"; // Giá trị mặc định hoặc giữ ảnh cũ nếu là Sửa

            if (input.FileAnhBia != null && input.FileAnhBia.Length > 0)
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "AnhBia");
                string originalFileName = Path.GetFileNameWithoutExtension(input.FileAnhBia.FileName);
                string extension = Path.GetExtension(input.FileAnhBia.FileName);
                string finalFileName = originalFileName + extension;
                string fullPath = Path.Combine(folderPath, finalFileName);
                int count = 1;
                while (System.IO.File.Exists(fullPath))
                {
                    finalFileName = $"{originalFileName}({count}){extension}";
                    fullPath = Path.Combine(folderPath, finalFileName);
                    count++;
                }
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await input.FileAnhBia.CopyToAsync(stream);
                }
                fileNameToSave = "/images/AnhBia/" + finalFileName;
            }

            truyen.MaTheLoais.Clear();
            if (input.SelectedTheLoais != null)
            {
                var listTheLoai = await _context.TheLoais.Where(tl => input.SelectedTheLoais.Contains(tl.MaTheLoai)).ToListAsync();
                truyen.MaTheLoais = listTheLoai;
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
        // --- ĐĂNG CHƯƠNG ---
        [HttpPost("dang-chuong")]
        public async Task<IActionResult> PostDangChuong([FromForm] ChuongTruyenDto input)
        {
            try
            {
                // 1. Thêm chương mới
                var chuong = new ChuongTruyen
                {
                    MaTruyen = input.MaTruyen,
                    TieuDe = input.TieuDe,
                    NoiDung = input.NoiDung,
                    ThuTuChuong = input.ThuTuChuong,
                    NgayDang = DateTime.Now
                };
                _context.ChuongTruyens.Add(chuong);

                // 2. Cập nhật thông tin truyện
                var truyen = await _context.Truyens.FindAsync(input.MaTruyen);
                if (truyen != null)
                {
                    truyen.NgayCapNhat = DateTime.Now;
                    truyen.SoChuong = await _context.ChuongTruyens.CountAsync(x => x.MaTruyen == input.MaTruyen) + 1;

                    // gửi thông báo cho những người đã theo dõi truyện
                    var nguoiTheoDois = await _context.TheoDois
                        .Where(td => td.MaTruyen == input.MaTruyen)
                        .Select(td => td.TaiKhoan)
                        .ToListAsync();

                    if (nguoiTheoDois.Any())
                    {
                        var listThongBao = new List<ThongBao>();
                        string noiDungTB = $"Truyện '{truyen.TenTruyen}' đã đăng chương mới: {chuong.TieuDe}";

                        foreach (var tk in nguoiTheoDois)
                        {
                            // Không gửi thông báo cho chính người đăng (nếu tác giả tự theo dõi truyện mình)
                            if (tk != truyen.NguoiDang)
                            {
                                listThongBao.Add(new ThongBao
                                {
                                    TaiKhoan = tk,
                                    NoiDung = noiDungTB,
                                    DaDoc = false,
                                    NgayGui = DateTime.Now
                                });
                            }
                        }

                        if (listThongBao.Count > 0)
                        {
                            _context.ThongBaos.AddRange(listThongBao);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // --- SỬA CHƯƠNG ---
        [HttpPut("sua-chuong")]
        public async Task<IActionResult> PutSuaChuong([FromForm] ChuongTruyenDto input)
        {
            var chuong = await _context.ChuongTruyens.FindAsync(input.MaChuongTruyen);
            if (chuong == null) return NotFound();
            chuong.TieuDe = input.TieuDe;
            chuong.NoiDung = input.NoiDung;
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
        [HttpPost("update-avatar")]
        public async Task<IActionResult> UpdateAvatar([FromForm] IFormFile file, [FromForm] string userName)
        {
            if (file == null || file.Length == 0) return BadRequest("File không hợp lệ");

            try
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "NguoiDung", "images", "Avatar");
                if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                string fileName = userName + "_" + DateTime.Now.Ticks + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(folderPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var user = _context.ThanhViens.FirstOrDefault(u => u.TaiKhoan == userName);
                if (user != null)
                {
                    string dbPath = "/NguoiDung/images/Avatar/" + fileName;
                    user.Avatar = dbPath;
                    await _context.SaveChangesAsync();

                    return Ok(new { success = true, newUrl = dbPath });
                }

                return NotFound(new { success = false, message = "Không tìm thấy người dùng" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet("GetFollowers/{maTruyen}")]
        public async Task<IActionResult> GetFollowers(int maTruyen)
        {
            var data = await _context.TheoDois
                .Where(x => x.MaTruyen == maTruyen)
                .OrderByDescending(x => x.NgayTheoDoi)
                .Select(x => new TuongTacDto
                {
                    TaiKhoan = x.TaiKhoan,
                    // Join thủ công hoặc lấy từ quan hệ nếu có Navigation Property
                    Avatar = _context.ThanhViens.FirstOrDefault(u => u.TaiKhoan == x.TaiKhoan)!.Avatar,
                    NgayThucHien = x.NgayTheoDoi.HasValue ? x.NgayTheoDoi.Value.ToString("dd/MM/yyyy HH:mm") : ""
                }).ToListAsync();
            return Ok(data);
        }

        [HttpGet("GetSavers/{maTruyen}")]
        public async Task<IActionResult> GetSavers(int maTruyen)
        {
            var data = await _context.LuuTruyens
                .Where(x => x.MaTruyen == maTruyen)
                .OrderByDescending(x => x.NgayLuu) // Giả sử có trường NgayLuu
                .Select(x => new TuongTacDto
                {
                    TaiKhoan = x.TaiKhoan,
                    Avatar = _context.ThanhViens.FirstOrDefault(u => u.TaiKhoan == x.TaiKhoan)!.Avatar,
                    NgayThucHien = x.NgayLuu.HasValue ? x.NgayLuu.Value.ToString("dd/MM/yyyy HH:mm") : ""
                }).ToListAsync();
            return Ok(data);
        }

        [HttpGet("GetRatings/{maTruyen}")]
        public async Task<IActionResult> GetRatings(int maTruyen)
        {
            var data = await _context.DanhGia
                .Where(x => x.MaTruyen == maTruyen)
                .OrderByDescending(x => x.NgayDanhGia)
                .Select(x => new DanhGiaDto
                {
                    TaiKhoan = x.TaiKhoan,
                    Avatar = _context.ThanhViens.FirstOrDefault(u => u.TaiKhoan == x.TaiKhoan)!.Avatar,
                    Diem = x.Diem ?? 0,
                    NoiDung = x.NoiDung,
                    NgayDanhGia = x.NgayDanhGia
                }).ToListAsync();
            return Ok(data);
        }

        [HttpGet("GetComments/{maTruyen}")]
        public async Task<IActionResult> GetComments(int maTruyen)
        {
            // Lấy tất cả bình luận thuộc các chương của truyện này
            var comments = await (from bl in _context.BinhLuans
                                  join c in _context.ChuongTruyens on bl.MaChuongTruyen equals c.MaChuongTruyen
                                  join u in _context.ThanhViens on bl.TaiKhoan equals u.TaiKhoan
                                  where c.MaTruyen == maTruyen
                                  orderby bl.NgayGui descending
                                  select new BinhLuanDto
                                  {
                                      MaBinhLuan = bl.MaBinhLuan,
                                      NoiDung = bl.NoiDung,
                                      TaiKhoan = bl.TaiKhoan,
                                      Avatar = u.Avatar,
                                      TenChuong = c.TieuDe,
                                      NgayGui = bl.NgayGui,
                                      // Lấy danh sách trả lời (Sub-query)
                                      RepBinhLuans = _context.RepBinhLuans
                                          .Where(r => r.MaBinhLuan == bl.MaBinhLuan)
                                          .OrderBy(r => r.NgayGui)
                                          .Select(r => new RepBinhLuanDto
                                          {
                                              MaRep = r.MaRep,
                                              TaiKhoan = r.TaiKhoan,
                                              Avatar = _context.ThanhViens.FirstOrDefault(uv => uv.TaiKhoan == r.TaiKhoan)!.Avatar,
                                              NoiDung = r.NoiDung,
                                              NgayGui = r.NgayGui
                                          }).ToList()
                                  }).ToListAsync();

            return Ok(comments);
        }
    }
}
