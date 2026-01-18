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
    public class ThanhViensController : Controller
    {
        private readonly WebTruyenChuContext _context;

        public ThanhViensController(WebTruyenChuContext context)
        {
            _context = context;
        }

        // GET: Admin/ThanhViens
        public async Task<IActionResult> Index()
        {
            return View(await _context.ThanhViens.ToListAsync());
        }

        // GET: Admin/ThanhViens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thanhVien = await _context.ThanhViens
                .FirstOrDefaultAsync(m => m.TaiKhoan == id);
            if (thanhVien == null)
            {
                return NotFound();
            }

            return View(thanhVien);
        }

        // GET: Admin/ThanhViens/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ThanhViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaiKhoan,MatKhau,Avatar,NgayTao,IsDeleted")] ThanhVien thanhVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thanhVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(thanhVien);
        }

        // GET: Admin/ThanhViens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thanhVien = await _context.ThanhViens.FindAsync(id);
            if (thanhVien == null)
            {
                return NotFound();
            }
            return View(thanhVien);
        }

        // POST: Admin/ThanhViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TaiKhoan,MatKhau,Avatar,NgayTao,IsDeleted")] ThanhVien thanhVien)
        {
            if (id != thanhVien.TaiKhoan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thanhVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThanhVienExists(thanhVien.TaiKhoan))
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
            return View(thanhVien);
        }

        // GET: Admin/ThanhViens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thanhVien = await _context.ThanhViens
                .FirstOrDefaultAsync(m => m.TaiKhoan == id);
            if (thanhVien == null)
            {
                return NotFound();
            }

            return View(thanhVien);
        }

        // POST: Admin/ThanhViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var thanhVien = await _context.ThanhViens.FindAsync(id);
            if (thanhVien != null)
            {
                _context.ThanhViens.Remove(thanhVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThanhVienExists(string id)
        {
            return _context.ThanhViens.Any(e => e.TaiKhoan == id);
        }
    }
}
