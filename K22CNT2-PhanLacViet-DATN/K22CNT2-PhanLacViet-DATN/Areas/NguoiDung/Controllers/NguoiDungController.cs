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
    }
}
