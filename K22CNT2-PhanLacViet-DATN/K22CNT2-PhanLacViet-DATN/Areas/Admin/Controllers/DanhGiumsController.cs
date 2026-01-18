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
            var webTruyenChuContext = _context.DanhGia.Include(d => d.MaTruyenNavigation).Include(d => d.TaiKhoanNavigation);
            return View(await webTruyenChuContext.ToListAsync());
        }

        // GET: Admin/DanhGiums/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGium = await _context.DanhGia
                .Include(d => d.MaTruyenNavigation)
                .Include(d => d.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == id);
            if (danhGium == null)
            {
                return NotFound();
            }

            return View(danhGium);
        }

        // GET: Admin/DanhGiums/Create
        public IActionResult Create()
        {
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen");
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan");
            return View();
        }

        // POST: Admin/DanhGiums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaiKhoan,MaTruyen,Diem,NoiDung,NgayDanhGia")] DanhGium danhGium)
        {
            if (ModelState.IsValid)
            {
                _context.Add(danhGium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", danhGium.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", danhGium.TaiKhoan);
            return View(danhGium);
        }

        // GET: Admin/DanhGiums/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGium = await _context.DanhGia.FindAsync(id);
            if (danhGium == null)
            {
                return NotFound();
            }
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", danhGium.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", danhGium.TaiKhoan);
            return View(danhGium);
        }

        // POST: Admin/DanhGiums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TaiKhoan,MaTruyen,Diem,NoiDung,NgayDanhGia")] DanhGium danhGium)
        {
            if (id != danhGium.TaiKhoan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(danhGium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DanhGiumExists(danhGium.TaiKhoan))
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
            ViewData["MaTruyen"] = new SelectList(_context.Truyens, "MaTruyen", "MaTruyen", danhGium.MaTruyen);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", danhGium.TaiKhoan);
            return View(danhGium);
        }

        // GET: Admin/DanhGiums/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhGium = await _context.DanhGia
                .Include(d => d.MaTruyenNavigation)
                .Include(d => d.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.TaiKhoan == id);
            if (danhGium == null)
            {
                return NotFound();
            }

            return View(danhGium);
        }

        // POST: Admin/DanhGiums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var danhGium = await _context.DanhGia.FindAsync(id);
            if (danhGium != null)
            {
                _context.DanhGia.Remove(danhGium);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DanhGiumExists(string id)
        {
            return _context.DanhGia.Any(e => e.TaiKhoan == id);
        }
    }
}
