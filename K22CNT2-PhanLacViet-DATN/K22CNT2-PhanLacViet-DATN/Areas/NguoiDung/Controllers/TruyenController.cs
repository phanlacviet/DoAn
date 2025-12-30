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
    public class TruyenController : Controller
    {
        private readonly WebTruyenChuContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        public TruyenController(WebTruyenChuContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("TruyenApi");
            var viewModel = new TrangChuViewModel();

            var taiKhoan = HttpContext.Session.GetString("USER_LOGIN");

            try
            {
                var resTruyen = await client.GetAsync("Truyen");
                if (resTruyen.IsSuccessStatusCode)
                {
                    viewModel.DanhSachTruyen = await resTruyen.Content.ReadFromJsonAsync<List<TruyenDto>>() ?? new List<TruyenDto>();
                }
                if (!string.IsNullOrEmpty(taiKhoan))
                {
                    var resLs = await client.GetAsync($"Truyen/LichSu/{taiKhoan}");
                    if (resLs.IsSuccessStatusCode)
                    {
                        viewModel.LichSu = await resLs.Content
                            .ReadFromJsonAsync<List<LichSuDocDto>>() ?? new List<LichSuDocDto>();
                    }
                    var resSave = await client.GetAsync($"Truyen/DaLuu/{taiKhoan}");
                    if (resSave.IsSuccessStatusCode)
                    {
                        viewModel.DaLuu = await resSave.Content.ReadFromJsonAsync<List<TruyenDto>>() ?? new List<TruyenDto>();
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi kết nối máy chủ API: " + ex.Message;
            }

            return View("TrangChu", viewModel);
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
        [HttpGet]
        public async Task<IActionResult> ChiTiet(int id)
        {
            var client = _httpClientFactory.CreateClient("TruyenApi");
            var viewModel = new TrangTruyenViewModel();
            var taiKhoan = HttpContext.Session.GetString("USER_LOGIN");
            try
            {
                string url = $"Truyen/ChiTiet/{id}";
                if (!string.IsNullOrEmpty(taiKhoan))
                {
                    url += $"?taiKhoan={taiKhoan}";
                }
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<TrangTruyenViewModel>();
                    if (data != null)
                    {
                        viewModel = data;
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi: " + ex.Message;
            }
            return View("ChiTiet", viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> ChiTietChuong(int id)
        {
            var taiKhoan = HttpContext.Session.GetString("USER_LOGIN") ?? "";
            var client = _httpClientFactory.CreateClient("TruyenApi");
            var viewModel = new ChiTietChuongViewModel();

            try
            {
                var response = await client.GetAsync($"Truyen/Chuong/{id}/{taiKhoan}");
                if (response.IsSuccessStatusCode)
                {
                    viewModel = await response.Content.ReadFromJsonAsync<ChiTietChuongViewModel>();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }
    }
}
