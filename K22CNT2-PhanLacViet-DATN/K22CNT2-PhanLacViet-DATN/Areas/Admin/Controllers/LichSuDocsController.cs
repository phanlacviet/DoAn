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
            var webTruyenChuContext = _context.LichSuDocs
                .Include(l => l.MaChuongTruyenNavigation)
                .Include(l => l.MaTruyenNavigation)
                .Include(l => l.TaiKhoanNavigation)
                .OrderByDescending(l => l.NgayDoc);
            return View(await webTruyenChuContext.ToListAsync());
        }

        // GET: Admin/LichSuDocs/Details?taiKhoan=abc&maTruyen=1
        public async Task<IActionResult> Details(string taiKhoan, int? maTruyen)
        {
            if (taiKhoan == null || maTruyen == null) return NotFound();

            var lichSuDoc = await _context.LichSuDocs
                .Include(l => l.MaChuongTruyenNavigation)
                .Include(l => l.MaTruyenNavigation)
                .Include(l => l.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == taiKhoan && m.MaTruyen == maTruyen);

            if (lichSuDoc == null) return NotFound();

            return View(lichSuDoc);
        }

        // GET: Admin/LichSuDocs/Create
        public IActionResult Create()
        {
            ViewData["MaChuongTruyen"] = new SelectList(_context.ChuongTruyens, "MaChuongTruyen", "TieuDe");
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "TenTruyen");
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaiKhoan,MaTruyen,MaChuongTruyen")] LichSuDoc lichSuDoc)
        {
            ModelState.Remove("MaChuongTruyenNavigation");
            ModelState.Remove("MaTruyenNavigation");
            ModelState.Remove("TaiKhoanNavigation");
            bool exists = _context.LichSuDocs.Any(l => l.TaiKhoan == lichSuDoc.TaiKhoan && l.MaTruyen == lichSuDoc.MaTruyen);

            if (exists)
            {
                ModelState.AddModelError("", "Lịch sử đọc của tài khoản này cho truyện này đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                lichSuDoc.NgayDoc = DateTime.Now;
                _context.Add(lichSuDoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MaChuongTruyen"] = new SelectList(_context.ChuongTruyens, "MaChuongTruyen", "TieuDe", lichSuDoc.MaChuongTruyen);
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "TenTruyen", lichSuDoc.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", lichSuDoc.TaiKhoan);
            return View(lichSuDoc);
        }

        // GET: Admin/LichSuDocs/Edit?taiKhoan=abc&maTruyen=1
        public async Task<IActionResult> Edit(string taiKhoan, int? maTruyen)
        {
            if (taiKhoan == null || maTruyen == null) return NotFound();

            // FindAsync có thể nhận nhiều tham số cho Composite Key
            var lichSuDoc = await _context.LichSuDocs.FindAsync(taiKhoan, maTruyen);

            if (lichSuDoc == null) return NotFound();
            ViewData["MaChuongTruyen"] = new SelectList(
                _context.ChuongTruyens.Where(c => c.MaTruyen == maTruyen),
                "MaChuongTruyen",
                "TieuDe",
                lichSuDoc.MaChuongTruyen
            );
            ViewData["MaChuongTruyen"] = new SelectList(_context.ChuongTruyens, "MaChuongTruyen", "TieuDe", lichSuDoc.MaChuongTruyen);
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "TenTruyen", lichSuDoc.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", lichSuDoc.TaiKhoan);
            return View(lichSuDoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string taiKhoan, int maTruyen, [Bind("TaiKhoan,MaTruyen,MaChuongTruyen,NgayDoc")] LichSuDoc lichSuDoc)
        {
            if (taiKhoan != lichSuDoc.TaiKhoan || maTruyen != lichSuDoc.MaTruyen)
            {
                return NotFound();
            }
            ModelState.Remove("MaChuongTruyenNavigation");
            ModelState.Remove("MaTruyenNavigation");
            ModelState.Remove("TaiKhoanNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichSuDoc);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichSuDocExists(lichSuDoc.TaiKhoan, lichSuDoc.MaTruyen)) return NotFound();
                    else throw;
                }
            }
            ViewData["MaChuongTruyen"] = new SelectList(_context.ChuongTruyens.Where(c => c.MaTruyen == maTruyen), "MaChuongTruyen", "TieuDe", lichSuDoc.MaChuongTruyen);
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "TenTruyen", lichSuDoc.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", lichSuDoc.TaiKhoan);
            return View(lichSuDoc);
        }

        // GET: Admin/LichSuDocs/Delete?taiKhoan=abc&maTruyen=1
        public async Task<IActionResult> Delete(string taiKhoan, int? maTruyen)
        {
            if (taiKhoan == null || maTruyen == null) return NotFound();

            var lichSuDoc = await _context.LichSuDocs
                .Include(l => l.MaChuongTruyenNavigation)
                .Include(l => l.MaTruyenNavigation)
                .Include(l => l.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == taiKhoan && m.MaTruyen == maTruyen);

            if (lichSuDoc == null) return NotFound();

            return View(lichSuDoc);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string taiKhoan, int maTruyen)
        {
            var lichSuDoc = await _context.LichSuDocs.FindAsync(taiKhoan, maTruyen);
            if (lichSuDoc != null)
            {
                _context.LichSuDocs.Remove(lichSuDoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<JsonResult> GetChaptersByStory(int maTruyen)
        {
            var chapters = await _context.ChuongTruyens
                .Where(c => c.MaTruyen == maTruyen)
                .OrderBy(c => c.ThuTuChuong)
                .Select(c => new {
                    value = c.MaChuongTruyen,
                    text = c.TieuDe
                })
                .ToListAsync();

            return Json(chapters);
        }
        private bool LichSuDocExists(string taiKhoan, int maTruyen)
        {
            return _context.LichSuDocs.Any(e => e.TaiKhoan == taiKhoan && e.MaTruyen == maTruyen);
        }
    }
}