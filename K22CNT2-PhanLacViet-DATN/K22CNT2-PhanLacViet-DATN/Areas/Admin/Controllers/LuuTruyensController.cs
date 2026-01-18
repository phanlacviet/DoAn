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
            var webTruyenChuContext = _context.LuuTruyens.Include(l => l.MaTruyenNavigation).Include(l => l.TaiKhoanNavigation);
            return View(await webTruyenChuContext.ToListAsync());
        }

        // GET: Admin/LuuTruyens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luuTruyen = await _context.LuuTruyens
                .Include(l => l.MaTruyenNavigation)
                .Include(l => l.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == id);
            if (luuTruyen == null)
            {
                return NotFound();
            }

            return View(luuTruyen);
        }

        // GET: Admin/LuuTruyens/Create
        public IActionResult Create()
        {
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen");
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan");
            return View();
        }

        // POST: Admin/LuuTruyens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaiKhoan,MaTruyen,NgayLuu")] LuuTruyen luuTruyen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(luuTruyen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", luuTruyen.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", luuTruyen.TaiKhoan);
            return View(luuTruyen);
        }

        // GET: Admin/LuuTruyens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luuTruyen = await _context.LuuTruyens.FindAsync(id);
            if (luuTruyen == null)
            {
                return NotFound();
            }
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", luuTruyen.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", luuTruyen.TaiKhoan);
            return View(luuTruyen);
        }

        // POST: Admin/LuuTruyens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TaiKhoan,MaTruyen,NgayLuu")] LuuTruyen luuTruyen)
        {
            if (id != luuTruyen.TaiKhoan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(luuTruyen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LuuTruyenExists(luuTruyen.TaiKhoan))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", luuTruyen.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", luuTruyen.TaiKhoan);
            return View(luuTruyen);
        }

        // GET: Admin/LuuTruyens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var luuTruyen = await _context.LuuTruyens
                .Include(l => l.MaTruyenNavigation)
                .Include(l => l.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == id);
            if (luuTruyen == null)
            {
                return NotFound();
            }

            return View(luuTruyen);
        }

        // POST: Admin/LuuTruyens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var luuTruyen = await _context.LuuTruyens.FindAsync(id);
            if (luuTruyen != null)
            {
                _context.LuuTruyens.Remove(luuTruyen);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LuuTruyenExists(string id)
        {
            return _context.LuuTruyens.Any(e => e.TaiKhoan == id);
        }
    }
}
