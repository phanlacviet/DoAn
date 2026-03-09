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
                    viewModel.DanhSachTruyen = await resTruyen.Content.ReadFromJsonAsync<List<TruyenDto>>() ?? new List<TruyenDto>();
                if (!string.IsNullOrEmpty(taiKhoan))
                {
                    var resLs = await client.GetAsync($"Truyen/LichSu/{taiKhoan}");
                    if (resLs.IsSuccessStatusCode)
                        viewModel.LichSu = await resLs.Content.ReadFromJsonAsync<List<LichSuDocDto>>() ?? new List<LichSuDocDto>();

                    var resSave = await client.GetAsync($"Truyen/DaLuu/{taiKhoan}");
                    if (resSave.IsSuccessStatusCode)
                        viewModel.DaLuu = await resSave.Content.ReadFromJsonAsync<List<TruyenDto>>() ?? new List<TruyenDto>();
                }
                var taskNgay = client.GetAsync("Truyen/BangXepHang/ngay");
                var taskTuan = client.GetAsync("Truyen/BangXepHang/tuan");
                var taskThang = client.GetAsync("Truyen/BangXepHang/thang");

                await Task.WhenAll(taskNgay, taskTuan, taskThang);
                if (taskNgay.Result.IsSuccessStatusCode)
                    viewModel.TopNgay = await taskNgay.Result.Content.ReadFromJsonAsync<List<TruyenDto>>() ?? new List<TruyenDto>();

                if (taskTuan.Result.IsSuccessStatusCode)
                    viewModel.TopTuan = await taskTuan.Result.Content.ReadFromJsonAsync<List<TruyenDto>>() ?? new List<TruyenDto>();

                if (taskThang.Result.IsSuccessStatusCode)
                    viewModel.TopThang = await taskThang.Result.Content.ReadFromJsonAsync<List<TruyenDto>>() ?? new List<TruyenDto>();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi kết nối: " + ex.Message;
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
            // 1. Kiểm tra trong bảng Quản trị trước
            var admin = await _context.QuanTris
                .FirstOrDefaultAsync(x => x.TaiKhoanQt == taiKhoan && x.MatKhau == matKhau);

            if (admin != null)
            {
                // Tạo Session riêng cho Admin
                HttpContext.Session.SetString("ADMIN_LOGIN", admin.TaiKhoanQt);
                // Chuyển hướng sang Area Admin
                return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
            }

            // 2. Nếu không phải Admin, kiểm tra trong bảng Thành viên
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

            // Tạo Session cho người dùng thường
            HttpContext.Session.SetString("USER_LOGIN", user.TaiKhoan);
            return RedirectToAction("Index", "Truyen"); 
        }

        [HttpPost]
        public async Task<IActionResult> Register(string taiKhoan, string matKhau)
        {
            // 1. Kiểm tra xem tài khoản đã tồn tại trong bảng Thành viên chưa
            bool isUserExist = await _context.ThanhViens.AnyAsync(x => x.TaiKhoan == taiKhoan);

            // 2. Kiểm tra xem tài khoản có trùng với tài khoản Quản trị không
            bool isAdminExist = await _context.QuanTris.AnyAsync(x => x.TaiKhoanQt == taiKhoan);

            if (isUserExist || isAdminExist)
            {
                ViewBag.Error = "Tài khoản này đã tồn tại hoặc không thể đăng ký";
                ViewBag.Mode = "register";
                return View("Auth");
            }

            // 3. Tiến hành đăng ký nếu hợp lệ
            var tv = new ThanhVien
            {
                TaiKhoan = taiKhoan,
                MatKhau = matKhau,
                NgayTao = DateTime.Now,
                IsDeleted = false
            };

            _context.ThanhViens.Add(tv);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("USER_LOGIN", taiKhoan);
            return RedirectToAction("Index", "Truyen");
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
                string accountParam = string.IsNullOrEmpty(taiKhoan) ? "guest" : taiKhoan;
                var response = await client.GetAsync($"Truyen/Chuong/{id}/{accountParam}");
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
        [HttpGet]
        public async Task<IActionResult> Search(
        string keyword, string tacGia, string moTa,
        string loaiTruyen, string luotXemRange, // Tham số mới
        string sort, List<int> genres)
        {
            var client = _httpClientFactory.CreateClient("TruyenApi");
            var viewModel = new TimKiemViewModel
            {
                Keyword = keyword,
                TacGia = tacGia,
                MoTa = moTa,
                LoaiTruyen = loaiTruyen,
                LuotXemRange = luotXemRange,
                Sort = sort,
                SelectedGenreIds = genres ?? new List<int>()
            };

            try
            {
                // Lấy danh sách thể loại để hiển thị bộ lọc
                var resTheLoai = await client.GetAsync("Truyen/TheLoai");
                if (resTheLoai.IsSuccessStatusCode)
                    viewModel.AllTheLoais = await resTheLoai.Content.ReadFromJsonAsync<List<TheLoaiItem>>() ?? new List<TheLoaiItem>();

                // Xây dựng QueryString
                var queryParams = new List<string>();
                if (!string.IsNullOrEmpty(keyword)) queryParams.Add($"keyword={keyword}");
                if (!string.IsNullOrEmpty(tacGia)) queryParams.Add($"tacGia={tacGia}");
                if (!string.IsNullOrEmpty(moTa)) queryParams.Add($"moTa={moTa}");
                if (!string.IsNullOrEmpty(loaiTruyen)) queryParams.Add($"loaiTruyen={loaiTruyen}");
                if (!string.IsNullOrEmpty(luotXemRange)) queryParams.Add($"luotXemRange={luotXemRange}");
                if (!string.IsNullOrEmpty(sort)) queryParams.Add($"sort={sort}");
                if (genres != null) foreach (var g in genres) queryParams.Add($"theLoais={g}");

                string queryString = string.Join("&", queryParams);
                var resSearch = await client.GetAsync($"Truyen/TimKiem?{queryString}");

                if (resSearch.IsSuccessStatusCode)
                    viewModel.KetQua = await resSearch.Content.ReadFromJsonAsync<List<TruyenDto>>() ?? new List<TruyenDto>();
            }
            catch (Exception ex) { ViewBag.Error = ex.Message; }

            return View(viewModel);
        }
        public async Task<IActionResult> DanhGia(int id)
        {
            var truyen = await _context.Truyens.FirstOrDefaultAsync(t => t.MaTruyen == id);
            if (truyen == null) return NotFound();
            var danhGias = await _context.DanhGia
                .Include(d => d.TaiKhoanNavigation)
                .Where(d => d.MaTruyen == id)
                .OrderByDescending(d => d.NgayDanhGia)
                .ToListAsync();
            var tongSo = danhGias.Count;
            var diemTB = tongSo > 0 ? danhGias.Average(d => (double)(d.Diem ?? 0)) : 0;
            var viewModel = new ReviewDetailViewModel
            {
                Truyen = truyen,
                DanhGias = danhGias,
                DiemTrungBinh = Math.Round(diemTB, 1),
                TongSoDanhGia = tongSo
            };

            return View(viewModel);
        }
    }
}
