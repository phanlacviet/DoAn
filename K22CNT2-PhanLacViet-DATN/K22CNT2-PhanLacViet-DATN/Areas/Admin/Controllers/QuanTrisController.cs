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
    public class QuanTrisController : Controller
    {
        private readonly WebTruyenChuContext _context;

        public QuanTrisController(WebTruyenChuContext context)
        {
            _context = context;
        }

        // GET: Admin/QuanTris
        public async Task<IActionResult> Index()
        {
            return View(await _context.QuanTris.ToListAsync());
        }

        // GET: Admin/QuanTris/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quanTri = await _context.QuanTris
                .FirstOrDefaultAsync(m => m.TaiKhoanQt == id);
            if (quanTri == null)
            {
                return NotFound();
            }

            return View(quanTri);
        }

        // GET: Admin/QuanTris/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/QuanTris/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaiKhoanQt,MatKhau")] QuanTri quanTri)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quanTri);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(quanTri);
        }

        // GET: Admin/QuanTris/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quanTri = await _context.QuanTris.FindAsync(id);
            if (quanTri == null)
            {
                return NotFound();
            }
            return View(quanTri);
        }

        // POST: Admin/QuanTris/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("TaiKhoanQt,MatKhau")] QuanTri quanTri)
        {
            if (id != quanTri.TaiKhoanQt)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quanTri);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuanTriExists(quanTri.TaiKhoanQt))
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
            return View(quanTri);
        }

        // GET: Admin/QuanTris/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var quanTri = await _context.QuanTris
                .FirstOrDefaultAsync(m => m.TaiKhoanQt == id);
            if (quanTri == null)
            {
                return NotFound();
            }

            return View(quanTri);
        }

        // POST: Admin/QuanTris/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var quanTri = await _context.QuanTris.FindAsync(id);
            if (quanTri != null)
            {
                _context.QuanTris.Remove(quanTri);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuanTriExists(string id)
        {
            return _context.QuanTris.Any(e => e.TaiKhoanQt == id);
        }
    }
}
