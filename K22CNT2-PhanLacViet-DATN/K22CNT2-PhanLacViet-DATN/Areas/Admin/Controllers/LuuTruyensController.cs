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
    public class LuuTruyensController : Controller
    {
        private readonly WebTruyenChuContext _context;

        public LuuTruyensController(WebTruyenChuContext context)
        {
            _context = context;
        }

        // GET: Admin/LuuTruyens
        public async Task<IActionResult> Index()
        {
            var webTruyenChuContext = _context.LuuTruyens
                .Include(l => l.MaTruyenNavigation)
                .Include(l => l.TaiKhoanNavigation)
                .OrderByDescending(l => l.NgayLuu);
            return View(await webTruyenChuContext.ToListAsync());
        }

        // GET: Admin/LuuTruyens/Details?taiKhoan=abc&maTruyen=1
        public async Task<IActionResult> Details(string taiKhoan, int? maTruyen)
        {
            if (taiKhoan == null || maTruyen == null) return NotFound();

            var luuTruyen = await _context.LuuTruyens
                .Include(l => l.MaTruyenNavigation)
                .Include(l => l.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == taiKhoan && m.MaTruyen == maTruyen);

            if (luuTruyen == null) return NotFound();

            return View(luuTruyen);
        }

        // GET: Admin/LuuTruyens/Create
        public IActionResult Create()
        {
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "TenTruyen");
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaiKhoan,MaTruyen")] LuuTruyen luuTruyen)
        {
            ModelState.Remove("MaTruyenNavigation");
            ModelState.Remove("TaiKhoanNavigation");

            bool exists = _context.LuuTruyens.Any(l => l.TaiKhoan == luuTruyen.TaiKhoan && l.MaTruyen == luuTruyen.MaTruyen);
            if (exists) ModelState.AddModelError("", "Truyện này đã được tài khoản lưu trước đó.");

            if (ModelState.IsValid)
            {
                luuTruyen.NgayLuu = DateTime.Now;
                _context.Add(luuTruyen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "TenTruyen", luuTruyen.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", luuTruyen.TaiKhoan);
            return View(luuTruyen);
        }

        // GET: Admin/LuuTruyens/Edit?taiKhoan=abc&maTruyen=1
        public async Task<IActionResult> Edit(string taiKhoan, int? maTruyen)
        {
            if (taiKhoan == null || maTruyen == null) return NotFound();

            var luuTruyen = await _context.LuuTruyens
                .Include(l => l.MaTruyenNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == taiKhoan && m.MaTruyen == maTruyen);

            if (luuTruyen == null) return NotFound();

            return View(luuTruyen);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string taiKhoan, int maTruyen, [Bind("TaiKhoan,MaTruyen,NgayLuu")] LuuTruyen luuTruyen)
        {
            if (taiKhoan != luuTruyen.TaiKhoan || maTruyen != luuTruyen.MaTruyen) return NotFound();

            ModelState.Remove("MaTruyenNavigation");
            ModelState.Remove("TaiKhoanNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(luuTruyen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LuuTruyenExists(luuTruyen.TaiKhoan, luuTruyen.MaTruyen)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(luuTruyen);
        }

        // GET: Admin/LuuTruyens/Delete?taiKhoan=abc&maTruyen=1
        public async Task<IActionResult> Delete(string taiKhoan, int? maTruyen)
        {
            if (taiKhoan == null || maTruyen == null) return NotFound();

            var luuTruyen = await _context.LuuTruyens
                .Include(l => l.MaTruyenNavigation)
                .Include(l => l.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == taiKhoan && m.MaTruyen == maTruyen);

            if (luuTruyen == null) return NotFound();

            return View(luuTruyen);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string taiKhoan, int maTruyen)
        {
            var luuTruyen = await _context.LuuTruyens.FindAsync(taiKhoan, maTruyen);
            if (luuTruyen != null)
            {
                _context.LuuTruyens.Remove(luuTruyen);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LuuTruyenExists(string taiKhoan, int maTruyen)
        {
            return _context.LuuTruyens.Any(e => e.TaiKhoan == taiKhoan && e.MaTruyen == maTruyen);
        }
    }
}