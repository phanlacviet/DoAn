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
    public class LichSuDocsController : Controller
    {
        private readonly WebTruyenChuContext _context;

        public LichSuDocsController(WebTruyenChuContext context)
        {
            _context = context;
        }

        // GET: Admin/LichSuDocs
        public async Task<IActionResult> Index()
        {
            var webTruyenChuContext = _context.LichSuDocs.Include(l => l.MaChuongTruyenNavigation).Include(l => l.MaTruyenNavigation).Include(l => l.TaiKhoanNavigation);
            return View(await webTruyenChuContext.ToListAsync());
        }

        // GET: Admin/LichSuDocs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichSuDoc = await _context.LichSuDocs
                .Include(l => l.MaChuongTruyenNavigation)
                .Include(l => l.MaTruyenNavigation)
                .Include(l => l.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == id);
            if (lichSuDoc == null)
            {
                return NotFound();
            }

            return View(lichSuDoc);
        }

        // GET: Admin/LichSuDocs/Create
        public IActionResult Create()
        {
            ViewData["MaChuongTruyen"] = new SelectList(_context.ChuongTruyens, "MaChuongTruyen", "MaChuongTruyen");
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen");
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan");
            return View();
        }

        // POST: Admin/LichSuDocs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaiKhoan,MaTruyen,MaChuongTruyen,NgayDoc")] LichSuDoc lichSuDoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lichSuDoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaChuongTruyen"] = new SelectList(_context.ChuongTruyens, "MaChuongTruyen", "MaChuongTruyen", lichSuDoc.MaChuongTruyen);
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", lichSuDoc.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", lichSuDoc.TaiKhoan);
            return View(lichSuDoc);
        }

        // GET: Admin/LichSuDocs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichSuDoc = await _context.LichSuDocs.FindAsync(id);
            if (lichSuDoc == null)
            {
                return NotFound();
            }
            ViewData["MaChuongTruyen"] = new SelectList(_context.ChuongTruyens, "MaChuongTruyen", "MaChuongTruyen", lichSuDoc.MaChuongTruyen);
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", lichSuDoc.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", lichSuDoc.TaiKhoan);
            return View(lichSuDoc);
        }

        // POST: Admin/LichSuDocs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TaiKhoan,MaTruyen,MaChuongTruyen,NgayDoc")] LichSuDoc lichSuDoc)
        {
            if (id != lichSuDoc.TaiKhoan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichSuDoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichSuDocExists(lichSuDoc.TaiKhoan))
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
            ViewData["MaChuongTruyen"] = new SelectList(_context.ChuongTruyens, "MaChuongTruyen", "MaChuongTruyen", lichSuDoc.MaChuongTruyen);
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", lichSuDoc.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", lichSuDoc.TaiKhoan);
            return View(lichSuDoc);
        }

        // GET: Admin/LichSuDocs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lichSuDoc = await _context.LichSuDocs
                .Include(l => l.MaChuongTruyenNavigation)
                .Include(l => l.MaTruyenNavigation)
                .Include(l => l.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == id);
            if (lichSuDoc == null)
            {
                return NotFound();
            }

            return View(lichSuDoc);
        }

        // POST: Admin/LichSuDocs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var lichSuDoc = await _context.LichSuDocs.FindAsync(id);
            if (lichSuDoc != null)
            {
                _context.LichSuDocs.Remove(lichSuDoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LichSuDocExists(string id)
        {
            return _context.LichSuDocs.Any(e => e.TaiKhoan == id);
        }
    }
}
