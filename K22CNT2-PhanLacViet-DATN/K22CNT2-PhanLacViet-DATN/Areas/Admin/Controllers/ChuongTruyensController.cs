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
    public class ChuongTruyensController : Controller
    {
        private readonly WebTruyenChuContext _context;

        public ChuongTruyensController(WebTruyenChuContext context)
        {
            _context = context;
        }

        // GET: Admin/ChuongTruyens
        public async Task<IActionResult> Index()
        {
            var webTruyenChuContext = _context.ChuongTruyens.Include(c => c.MaTruyenNavigation);
            return View(await webTruyenChuContext.ToListAsync());
        }

        // GET: Admin/ChuongTruyens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuongTruyen = await _context.ChuongTruyens
                .Include(c => c.MaTruyenNavigation)
                .FirstOrDefaultAsync(m => m.MaChuongTruyen == id);
            if (chuongTruyen == null)
            {
                return NotFound();
            }

            return View(chuongTruyen);
        }

        // GET: Admin/ChuongTruyens/Create
        public IActionResult Create()
        {
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen");
            return View();
        }

        // POST: Admin/ChuongTruyens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaChuongTruyen,MaTruyen,ThuTuChuong,TieuDe,NoiDung,NgayDang")] ChuongTruyen chuongTruyen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chuongTruyen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", chuongTruyen.MaTruyen);
            return View(chuongTruyen);
        }

        // GET: Admin/ChuongTruyens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuongTruyen = await _context.ChuongTruyens.FindAsync(id);
            if (chuongTruyen == null)
            {
                return NotFound();
            }
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", chuongTruyen.MaTruyen);
            return View(chuongTruyen);
        }

        // POST: Admin/ChuongTruyens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaChuongTruyen,MaTruyen,ThuTuChuong,TieuDe,NoiDung,NgayDang")] ChuongTruyen chuongTruyen)
        {
            if (id != chuongTruyen.MaChuongTruyen)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chuongTruyen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChuongTruyenExists(chuongTruyen.MaChuongTruyen))
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
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", chuongTruyen.MaTruyen);
            return View(chuongTruyen);
        }

        // GET: Admin/ChuongTruyens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuongTruyen = await _context.ChuongTruyens
                .Include(c => c.MaTruyenNavigation)
                .FirstOrDefaultAsync(m => m.MaChuongTruyen == id);
            if (chuongTruyen == null)
            {
                return NotFound();
            }

            return View(chuongTruyen);
        }

        // POST: Admin/ChuongTruyens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chuongTruyen = await _context.ChuongTruyens.FindAsync(id);
            if (chuongTruyen != null)
            {
                _context.ChuongTruyens.Remove(chuongTruyen);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChuongTruyenExists(int id)
        {
            return _context.ChuongTruyens.Any(e => e.MaChuongTruyen == id);
        }
    }
}
