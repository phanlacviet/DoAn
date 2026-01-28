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
    public class LuotXemTruyensController : Controller
    {
        private readonly WebTruyenChuContext _context;

        public LuotXemTruyensController(WebTruyenChuContext context)
        {
            _context = context;
        }

        // GET: Admin/LuotXemTruyens
        public async Task<IActionResult> Index()
        {
            var webTruyenChuContext = _context.LuotXemTruyens
                .Include(l => l.MaTruyenNavigation)
                .OrderByDescending(l => l.Ngay);
            return View(await webTruyenChuContext.ToListAsync());
        }

        // GET: Admin/LuotXemTruyens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var luotXemTruyen = await _context.LuotXemTruyens
                .Include(l => l.MaTruyenNavigation)
                .FirstOrDefaultAsync(m => m.MaLuotXem == id);

            if (luotXemTruyen == null) return NotFound();

            return View(luotXemTruyen);
        }

        // GET: Admin/LuotXemTruyens/Create
        public IActionResult Create()
        {
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "TenTruyen");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLuotXem,MaTruyen,Ngay,SoLuotXem")] LuotXemTruyen luotXemTruyen)
        {
            // Loại bỏ xác thực cho thuộc tính điều hướng
            ModelState.Remove("MaTruyenNavigation");

            if (ModelState.IsValid)
            {
                _context.Add(luotXemTruyen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "TenTruyen", luotXemTruyen.MaTruyen);
            return View(luotXemTruyen);
        }

        // GET: Admin/LuotXemTruyens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var luotXemTruyen = await _context.LuotXemTruyens.FindAsync(id);
            if (luotXemTruyen == null) return NotFound();

            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "TenTruyen", luotXemTruyen.MaTruyen);
            return View(luotXemTruyen);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLuotXem,MaTruyen,Ngay,SoLuotXem")] LuotXemTruyen luotXemTruyen)
        {
            if (id != luotXemTruyen.MaLuotXem) return NotFound();

            ModelState.Remove("MaTruyenNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(luotXemTruyen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LuotXemTruyenExists(luotXemTruyen.MaLuotXem)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "TenTruyen", luotXemTruyen.MaTruyen);
            return View(luotXemTruyen);
        }

        // GET: Admin/LuotXemTruyens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var luotXemTruyen = await _context.LuotXemTruyens
                .Include(l => l.MaTruyenNavigation)
                .FirstOrDefaultAsync(m => m.MaLuotXem == id);

            if (luotXemTruyen == null) return NotFound();

            return View(luotXemTruyen);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var luotXemTruyen = await _context.LuotXemTruyens.FindAsync(id);
            if (luotXemTruyen != null)
            {
                _context.LuotXemTruyens.Remove(luotXemTruyen);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LuotXemTruyenExists(int id)
        {
            return _context.LuotXemTruyens.Any(e => e.MaLuotXem == id);
        }
    }
}