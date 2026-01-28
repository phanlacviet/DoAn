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
    public class RepBinhLuansController : Controller
    {
        private readonly WebTruyenChuContext _context;

        public RepBinhLuansController(WebTruyenChuContext context)
        {
            _context = context;
        }

        // GET: Admin/RepBinhLuans
        public async Task<IActionResult> Index()
        {
            var webTruyenChuContext = _context.RepBinhLuans.Include(r => r.MaBinhLuanNavigation).Include(r => r.TaiKhoanNavigation);
            return View(await webTruyenChuContext.ToListAsync());
        }

        // GET: Admin/RepBinhLuans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repBinhLuan = await _context.RepBinhLuans
                .Include(r => r.MaBinhLuanNavigation)
                .Include(r => r.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.MaRep == id);
            if (repBinhLuan == null)
            {
                return NotFound();
            }

            return View(repBinhLuan);
        }

        // GET: Admin/RepBinhLuans/Create
        public IActionResult Create()
        {
            ViewData["MaBinhLuan"] = new SelectList(_context.BinhLuans, "MaBinhLuan", "MaBinhLuan");
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan");
            return View();
        }

        // POST: Admin/RepBinhLuans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaRep,MaBinhLuan,TaiKhoan,NoiDung")] RepBinhLuan repBinhLuan)
        {
            if (ModelState.IsValid)
            {
                repBinhLuan.NgayGui = DateTime.Now;
                _context.Add(repBinhLuan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaBinhLuan"] = new SelectList(_context.BinhLuans, "MaBinhLuan", "MaBinhLuan", repBinhLuan.MaBinhLuan);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", repBinhLuan.TaiKhoan);
            return View(repBinhLuan);
        }

        // GET: Admin/RepBinhLuans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repBinhLuan = await _context.RepBinhLuans.FindAsync(id);
            if (repBinhLuan == null)
            {
                return NotFound();
            }
            ViewData["MaBinhLuan"] = new SelectList(_context.BinhLuans, "MaBinhLuan", "MaBinhLuan", repBinhLuan.MaBinhLuan);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", repBinhLuan.TaiKhoan);
            return View(repBinhLuan);
        }

        // POST: Admin/RepBinhLuans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaRep,MaBinhLuan,TaiKhoan,NoiDung,NgayGui")] RepBinhLuan repBinhLuan)
        {
            if (id != repBinhLuan.MaRep)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(repBinhLuan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RepBinhLuanExists(repBinhLuan.MaRep))
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
            ViewData["MaBinhLuan"] = new SelectList(_context.BinhLuans, "MaBinhLuan", "MaBinhLuan", repBinhLuan.MaBinhLuan);
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", repBinhLuan.TaiKhoan);
            return View(repBinhLuan);
        }

        // GET: Admin/RepBinhLuans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repBinhLuan = await _context.RepBinhLuans
                .Include(r => r.MaBinhLuanNavigation)
                .Include(r => r.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.MaRep == id);
            if (repBinhLuan == null)
            {
                return NotFound();
            }

            return View(repBinhLuan);
        }

        // POST: Admin/RepBinhLuans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var repBinhLuan = await _context.RepBinhLuans.FindAsync(id);
            if (repBinhLuan != null)
            {
                _context.RepBinhLuans.Remove(repBinhLuan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RepBinhLuanExists(int id)
        {
            return _context.RepBinhLuans.Any(e => e.MaRep == id);
        }
    }
}
