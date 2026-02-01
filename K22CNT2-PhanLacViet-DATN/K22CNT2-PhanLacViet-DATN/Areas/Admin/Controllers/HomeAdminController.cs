using K22CNT2_PhanLacViet_DATN.Areas.Admin.Models;
using K22CNT2_PhanLacViet_DATN.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http; // Cần thiết để dùng HttpContext.Session

namespace K22CNT2_PhanLacViet_DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeAdminController : Controller
    {
        private readonly WebTruyenChuContext _context;

        public HomeAdminController(WebTruyenChuContext context)
        {
            _context = context;
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("ADMIN_LOGIN")))
            {
                // Lưu ý: Kiểm tra lại tên Action và Controller của trang đăng nhập của bạn
                context.Result = new RedirectToActionResult("Auth", "Truyen", new { area = "NguoiDung" });
            }
            base.OnActionExecuting(context);
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                TongThanhVien = _context.ThanhViens.Count(),
                TongTruyen = _context.Truyens.Count(),
                TongChuong = _context.ChuongTruyens.Count(),
                ThanhVienMoi7Ngay = _context.ThanhViens.Where(u => u.NgayTao >= DateTime.Now.AddDays(-7)).Count(),
                TopTruyenXemNhieu = _context.Truyens
                                            .OrderByDescending(t => t.TongLuotXem)
                                            .Take(5)
                                            .ToList(),
                TruyenMoiCapNhat = _context.Truyens
                                           .OrderByDescending(t => t.NgayCapNhat)
                                           .Take(5)
                                           .ToList()
            };
            var labels = new List<string>();
            var data = new List<int>();
            var listLuotXem = _context.LuotXemTruyens
                .Where(x => x.Ngay >= DateTime.Now.AddDays(-14))
                .GroupBy(x => x.Ngay)
                .Select(g => new {
                    Ngay = g.Key,
                    TongXem = g.Sum(s => s.SoLuotXem ?? 0)
                })
                .OrderBy(x => x.Ngay)
                .ToList();

            foreach (var item in listLuotXem)
            {
                labels.Add(item.Ngay?.ToString("dd/MM") ?? "");
                data.Add(item.TongXem);
            }
            model.ChartLabels = labels.ToArray();
            model.ChartData = data.ToArray();

            return View(model);
        }
    }
}