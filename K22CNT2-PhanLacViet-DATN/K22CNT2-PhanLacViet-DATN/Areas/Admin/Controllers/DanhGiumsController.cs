using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using K22CNT2_PhanLacViet_DATN.Models;

namespace K22CNT2_PhanLacViet_DATN.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DanhGiumsController : Controller
    {
        private readonly WebTruyenChuContext _context;

        public DanhGiumsController(WebTruyenChuContext context)
        {
            _context = context;
        }

        // GET: Admin/DanhGiums
        public async Task<IActionResult> Index()
        {
            var webTruyenChuContext = _context.DanhGia
                .Include(d => d.MaTruyenNavigation)
                .Include(d => d.TaiKhoanNavigation)
                .OrderByDescending(d => d.NgayDanhGia);
            return View(await webTruyenChuContext.ToListAsync());
        }

        // GET: Admin/DanhGiums/Details?taiKhoan=abc&maTruyen=1
        public async Task<IActionResult> Details(string taiKhoan, int? maTruyen)
        {
            if (taiKhoan == null || maTruyen == null) return NotFound();

            var danhGium = await _context.DanhGia
                .Include(d => d.MaTruyenNavigation)
                .Include(d => d.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == taiKhoan && m.MaTruyen == maTruyen);

            if (danhGium == null) return NotFound();

            return View(danhGium);
        }

        public IActionResult Create()
        {
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "TenTruyen");
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaiKhoan,MaTruyen,Diem,NoiDung")] DanhGium danhGium)
        {
            ModelState.Remove("MaTruyenNavigation");
            ModelState.Remove("TaiKhoanNavigation");
            if (ModelState.IsValid)
            {
                // Kiểm tra xem user này đã đánh giá truyện này chưa (tránh lỗi PK)
                bool exists = _context.DanhGia.Any(d => d.TaiKhoan == danhGium.TaiKhoan && d.MaTruyen == danhGium.MaTruyen);
                if (exists)
                {
                    ModelState.AddModelError("", "Người dùng này đã đánh giá truyện này rồi.");
                }
                else
                {
                    danhGium.NgayDanhGia = DateTime.Now;
                    _context.Add(danhGium);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "TenTruyen", danhGium.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", danhGium.TaiKhoan);
            return View(danhGium);
        }

        // GET: Admin/DanhGiums/Edit?taiKhoan=abc&maTruyen=1
        public async Task<IActionResult> Edit(string taiKhoan, int? maTruyen)
        {
            if (taiKhoan == null || maTruyen == null) return NotFound();
            var danhGium = await _context.DanhGia.FirstOrDefaultAsync(x => x.TaiKhoan == taiKhoan && x.MaTruyen == maTruyen);

            if (danhGium == null) return NotFound();

            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "TenTruyen", danhGium.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", danhGium.TaiKhoan);
            return View(danhGium);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string TaiKhoan, int MaTruyen, [Bind("TaiKhoan,MaTruyen,Diem,NoiDung,NgayDanhGia")] DanhGium danhGium)
        {
            // Kiểm tra khớp dữ liệu của cả 2 thành phần khóa chính
            if (TaiKhoan != danhGium.TaiKhoan || MaTruyen != danhGium.MaTruyen)
            {
                return NotFound();
            }
            ModelState.Remove("MaTruyenNavigation");
            ModelState.Remove("TaiKhoanNavigation");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danhGium);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhGiumExists(danhGium.TaiKhoan, danhGium.MaTruyen)) return NotFound();
                    else throw;
                }
            }
            
            return View(danhGium);
        }

        public async Task<IActionResult> Delete(string taiKhoan, int? maTruyen)
        {
            if (taiKhoan == null || maTruyen == null) return NotFound();

            var danhGium = await _context.DanhGia
                .Include(d => d.MaTruyenNavigation)
                .Include(d => d.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == taiKhoan && m.MaTruyen == maTruyen);

            if (danhGium == null) return NotFound();

            return View(danhGium);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string taiKhoan, int maTruyen)
        {
            var danhGium = await _context.DanhGia.FindAsync(taiKhoan, maTruyen);
            if (danhGium != null)
            {
                _context.DanhGia.Remove(danhGium);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DanhGiumExists(string taiKhoan, int maTruyen)
        {
            return _context.DanhGia.Any(e => e.TaiKhoan == taiKhoan && e.MaTruyen == maTruyen);
        }
    }
}