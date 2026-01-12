using K22CNT2_PhanLacViet_DATN.Models;
using K22CNT2_PhanLacViet_DATN.Models.Dtos;
using K22CNT2_PhanLacViet_DATN.Areas.NguoiDung.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;

namespace K22CNT2_PhanLacViet_DATN.Areas.NguoiDung.Controllers
{
    [Area("NguoiDung")]
    public class NguoiDungController : Controller
    {
        private readonly WebTruyenChuContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        public NguoiDungController(WebTruyenChuContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult ThongTinNguoiDUng()
        {
            var taiKhoan = HttpContext.Session.GetString("USER_LOGIN");
            if (string.IsNullOrEmpty(taiKhoan))
            {
                return RedirectToAction("DangNhap", "TaiKhoan", new { area = "NguoiDung" });
            }
            ViewBag.CurrentUser = taiKhoan;
            return View();
        }
        // GET: /NguoiDung/TacGiaDashboard
        // Xóa bỏ IHttpClientFactory không cần thiết
        public async Task<IActionResult> ThongTinTacGia()
        {
            var taiKhoan = HttpContext.Session.GetString("USER_LOGIN");
            if (string.IsNullOrEmpty(taiKhoan))
            {
                return RedirectToAction("Auth", "NguoiDung");
            }
            try
            {
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
                        LuotBinhLuan = _context.BinhLuans.Count(bl => bl.MaChuongTruyenNavigation.MaTruyen == t.MaTruyen),
                        DiemDanhGia = _context.DanhGia.Where(dg => dg.MaTruyen == t.MaTruyen).Average(dg => (double?)dg.Diem) ?? 0,
                        LuotDanhGia = _context.DanhGia.Count(dg => dg.MaTruyen == t.MaTruyen)
                    })
                    .OrderByDescending(t => t.NgayCapNhat)
                    .ToListAsync();

                // 2. Tính toán thống kê
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

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi hệ thống: " + ex.Message;
                return View(new TacGiaViewModels());
            }
        }
    }
}
