using K22CNT2_PhanLacViet_DATN.Models;
using K22CNT2_PhanLacViet_DATN.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace K22CNT2_PhanLacViet_DATN.Areas.NguoiDung.Controllers
{
    [Area("NguoiDung")]
    public class TruyenController : Controller
    {
        private readonly WebTruyenChuContext _context;

        public TruyenController(WebTruyenChuContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var truyen = await _context.Truyens
                .Where(t => t.IsDeleted == false)
                .OrderByDescending(t => t.NgayDang)
                .ToListAsync();
            return View("TrangChu", truyen);
        }

        [HttpGet]
        public IActionResult Auth(string mode = "login")
        {
            ViewBag.Mode = mode;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string taiKhoan, string matKhau)
        {
            var user = await _context.ThanhViens
                .FirstOrDefaultAsync(x =>
                    x.TaiKhoan == taiKhoan &&
                    x.MatKhau == matKhau &&
                    x.IsDeleted == false);

            if (user == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                ViewBag.Mode = "login";
                return View("Auth");
            }

            HttpContext.Session.SetString("USER_LOGIN", user.TaiKhoan);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Register(string taiKhoan, string matKhau)
        {
            if (await _context.ThanhViens.AnyAsync(x => x.TaiKhoan == taiKhoan))
            {
                ViewBag.Error = "Tài khoản đã tồn tại";
                ViewBag.Mode = "register";
                return View("Auth");
            }

            var tv = new ThanhVien
            {
                TaiKhoan = taiKhoan,
                MatKhau = matKhau
            };

            _context.ThanhViens.Add(tv);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("USER_LOGIN", taiKhoan);

            return RedirectToAction("Index");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("USER_LOGIN");
            return RedirectToAction("Index");
        }
    }
}
