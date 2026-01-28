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
    public class ThongBaosController : Controller
    {
        private readonly WebTruyenChuContext _context;

        public ThongBaosController(WebTruyenChuContext context)
        {
            _context = context;
        }

        // GET: Admin/ThongBaos
        public async Task<IActionResult> Index()
        {
            var webTruyenChuContext = _context.ThongBaos
                .Include(t => t.TaiKhoanNavigation)
                .OrderByDescending(t => t.NgayGui);
            return View(await webTruyenChuContext.ToListAsync());
        }

        // GET: Admin/ThongBaos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var thongBao = await _context.ThongBaos
                .Include(t => t.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.MaThongBao == id);

            if (thongBao == null) return NotFound();

            return View(thongBao);
        }

        // GET: Admin/ThongBaos/Create
        public IActionResult Create()
        {
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TaiKhoan,NoiDung")] ThongBao thongBao)
        {
            // Bỏ qua xác thực navigation
            ModelState.Remove("TaiKhoanNavigation");

            if (ModelState.IsValid)
            {
                thongBao.NgayGui = DateTime.Now;
                thongBao.DaDoc = false; // Mặc định là chưa đọc
                _context.Add(thongBao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", thongBao.TaiKhoan);
            return View(thongBao);
        }

        // GET: Admin/ThongBaos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var thongBao = await _context.ThongBaos.FindAsync(id);
            if (thongBao == null) return NotFound();

            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", thongBao.TaiKhoan);
            return View(thongBao);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaThongBao,TaiKhoan,NoiDung,DaDoc,NgayGui")] ThongBao thongBao)
        {
            if (id != thongBao.MaThongBao) return NotFound();

            ModelState.Remove("TaiKhoanNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thongBao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThongBaoExists(thongBao.MaThongBao)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TaiKhoan"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", thongBao.TaiKhoan);
            return View(thongBao);
        }

        // GET: Admin/ThongBaos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var thongBao = await _context.ThongBaos
                .Include(t => t.TaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.MaThongBao == id);

            if (thongBao == null) return NotFound();

            return View(thongBao);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var thongBao = await _context.ThongBaos.FindAsync(id);
            if (thongBao != null)
            {
                _context.ThongBaos.Remove(thongBao);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThongBaoExists(int id)
        {
            return _context.ThongBaos.Any(e => e.MaThongBao == id);
        }
    }
}