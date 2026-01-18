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
    public class TruyensController : Controller
    {
        private readonly WebTruyenChuContext _context;

        public TruyensController(WebTruyenChuContext context)
        {
            _context = context;
        }

        // GET: Admin/Truyens
        public async Task<IActionResult> Index()
        {
            var webTruyenChuContext = _context.Truyens.Include(t => t.NguoiDangNavigation);
            return View(await webTruyenChuContext.ToListAsync());
        }

        // GET: Admin/Truyens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var truyen = await _context.Truyens
                .Include(t => t.NguoiDangNavigation)
                .FirstOrDefaultAsync(m => m.MaTruyen == id);
            if (truyen == null)
            {
                return NotFound();
            }

            return View(truyen);
        }

        // GET: Admin/Truyens/Create
        public IActionResult Create()
        {
            ViewData["NguoiDang"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan");
            return View();
        }

        // POST: Admin/Truyens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTruyen,TenTruyen,MoTa,TacGia,NguoiDang,LoaiTruyen,SoChuong,TongLuotXem,NgayDang,NgayCapNhat,IsDeleted,AnhBia,TrangThai")] Truyen truyen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(truyen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NguoiDang"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", truyen.NguoiDang);
            return View(truyen);
        }

        // GET: Admin/Truyens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var truyen = await _context.Truyens.FindAsync(id);
            if (truyen == null)
            {
                return NotFound();
            }
            ViewData["NguoiDang"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", truyen.NguoiDang);
            return View(truyen);
        }

        // POST: Admin/Truyens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTruyen,TenTruyen,MoTa,TacGia,NguoiDang,LoaiTruyen,SoChuong,TongLuotXem,NgayDang,NgayCapNhat,IsDeleted,AnhBia,TrangThai")] Truyen truyen)
        {
            if (id != truyen.MaTruyen)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(truyen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TruyenExists(truyen.MaTruyen))
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
            ViewData["NguoiDang"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", truyen.NguoiDang);
            return View(truyen);
        }

        // GET: Admin/Truyens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var truyen = await _context.Truyens
                .Include(t => t.NguoiDangNavigation)
                .FirstOrDefaultAsync(m => m.MaTruyen == id);
            if (truyen == null)
            {
                return NotFound();
            }

            return View(truyen);
        }

        // POST: Admin/Truyens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var truyen = await _context.Truyens.FindAsync(id);
            if (truyen != null)
            {
                _context.Truyens.Remove(truyen);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TruyenExists(int id)
        {
            return _context.Truyens.Any(e => e.MaTruyen == id);
        }
    }
}
