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
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var theoDoi = await _context.TheoDois
                .Include(t => t.MaTruyenNavigation)
                .Include(t => t.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == id);
            if (theoDoi == null)
            {
                return NotFound();
            }

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
        public async Task<IActionResult> Create([Bind("TaiKhoan,MaTruyen,NgayTheoDoi")] TheoDoi theoDoi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(theoDoi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", theoDoi.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", theoDoi.TaiKhoan);
            return View(theoDoi);
        }

        // GET: Admin/TheoDois/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var theoDoi = await _context.TheoDois.FindAsync(id);
            if (theoDoi == null)
            {
                return NotFound();
            }
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", theoDoi.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", theoDoi.TaiKhoan);
            return View(theoDoi);
        }

        // POST: Admin/TheoDois/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TaiKhoan,MaTruyen,NgayTheoDoi")] TheoDoi theoDoi)
        {
            if (id != theoDoi.TaiKhoan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(theoDoi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TheoDoiExists(theoDoi.TaiKhoan))
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
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", theoDoi.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", theoDoi.TaiKhoan);
            return View(theoDoi);
        }

        // GET: Admin/TheoDois/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var theoDoi = await _context.TheoDois
                .Include(t => t.MaTruyenNavigation)
                .Include(t => t.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == id);
            if (theoDoi == null)
            {
                return NotFound();
            }

            return View(theoDoi);
        }

        // POST: Admin/TheoDois/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var theoDoi = await _context.TheoDois.FindAsync(id);
            if (theoDoi != null)
            {
                _context.TheoDois.Remove(theoDoi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TheoDoiExists(string id)
        {
            return _context.TheoDois.Any(e => e.TaiKhoan == id);
        }
    }
}
