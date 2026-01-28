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
    public class TheoDoisController : Controller
    {
        private readonly WebTruyenChuContext _context;

        public TheoDoisController(WebTruyenChuContext context)
        {
            _context = context;
        }

        // GET: Admin/TheoDois
        public async Task<IActionResult> Index()
        {
            var webTruyenChuContext = _context.TheoDois.Include(t => t.MaTruyenNavigation).Include(t => t.TaiKhoanNavigation);
            return View(await webTruyenChuContext.ToListAsync());
        }

        // GET: Admin/TheoDois/Details/5
        public async Task<IActionResult> Details(string taiKhoan, int? maTruyen)
        {
            if (taiKhoan == null || maTruyen == null) return NotFound();

            var theoDoi = await _context.TheoDois
                .Include(t => t.MaTruyenNavigation)
                .Include(t => t.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == taiKhoan && m.MaTruyen == maTruyen);

            if (theoDoi == null) return NotFound();

            return View(theoDoi);
        }

        // GET: Admin/TheoDois/Create
        public IActionResult Create()
        {
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen");
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan");
            return View();
        }

        // POST: Admin/TheoDois/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaiKhoan,MaTruyen")] TheoDoi theoDoi)
        {
            bool exists = _context.TheoDois.Any(t => t.TaiKhoan == theoDoi.TaiKhoan && t.MaTruyen == theoDoi.MaTruyen);
            ModelState.Remove("MaTruyenNavigation");
            ModelState.Remove("TaiKhoanNavigation");
            if (exists) ModelState.AddModelError("", "Tài khoản này đã theo dõi truyện này rồi.");
            if (ModelState.IsValid)
            {
                theoDoi.NgayTheoDoi = DateTime.Now;
                _context.Add(theoDoi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "TenTruyen", theoDoi.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", theoDoi.TaiKhoan);
            return View(theoDoi);
        }

        // GET: Admin/TheoDois/Edit/5
        public async Task<IActionResult> Edit(string taiKhoan, int? maTruyen)
        {
            if (taiKhoan == null || maTruyen == null) return NotFound();

            // Với Composite Key, phải truyền đủ 2 tham số vào FindAsync
            var theoDoi = await _context.TheoDois
                .Include(t => t.MaTruyenNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == taiKhoan && m.MaTruyen == maTruyen);

            if (theoDoi == null) return NotFound();

            return View(theoDoi);
        }

        // POST: Admin/TheoDois/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string taiKhoan, int maTruyen, [Bind("TaiKhoan,MaTruyen,NgayTheoDoi")] TheoDoi theoDoi)
        {
            if (taiKhoan != theoDoi.TaiKhoan || maTruyen != theoDoi.MaTruyen) return NotFound();

            ModelState.Remove("MaTruyenNavigation");
            ModelState.Remove("TaiKhoanNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(theoDoi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TheoDoiExists(theoDoi.TaiKhoan, theoDoi.MaTruyen)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(theoDoi);
        }

        // GET: Admin/TheoDois/Delete/5
        // GET: Admin/TheoDois/Delete?taiKhoan=abc&maTruyen=1
        public async Task<IActionResult> Delete(string taiKhoan, int? maTruyen)
        {
            if (taiKhoan == null || maTruyen == null)
            {
                return NotFound();
            }

            var theoDoi = await _context.TheoDois
                .Include(t => t.MaTruyenNavigation)
                .Include(t => t.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == taiKhoan && m.MaTruyen == maTruyen);

            if (theoDoi == null)
            {
                return NotFound();
            }

            return View(theoDoi);
        }

        // POST: Admin/TheoDois/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string taiKhoan, int maTruyen)
        {
            // Tìm chính xác bản ghi dựa trên cả 2 khóa
            var theoDoi = await _context.TheoDois.FindAsync(taiKhoan, maTruyen);

            if (theoDoi != null)
            {
                _context.TheoDois.Remove(theoDoi);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TheoDoiExists(string taiKhoan, int maTruyen)
        {
            return _context.TheoDois.Any(e => e.TaiKhoan == taiKhoan && e.MaTruyen == maTruyen);
        }
    }
}
