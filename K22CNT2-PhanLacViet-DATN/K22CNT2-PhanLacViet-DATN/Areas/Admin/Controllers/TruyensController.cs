using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _webHostEnvironment; 

        public TruyensController(WebTruyenChuContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
            ViewData["TheLoai"] = _context.TheLoais.Where(x => x.IsDeleted == false).ToList();
            return View();
        }

        // POST: Admin/Truyens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Truyen truyen, IFormFile AnhBiaFile, int[] selectedTheLoai)
        {
            if (ModelState.IsValid)
            {
                if (AnhBiaFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "AnhBia");
                    if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                    string fileName = AnhBiaFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, fileName);                    // Kiểm tra trùng tên file
                    if (System.IO.File.Exists(filePath))
                    {
                        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                        string extension = Path.GetExtension(fileName);
                        fileName = $"{fileNameWithoutExtension}_{DateTime.Now.Ticks}{extension}";
                        filePath = Path.Combine(uploadsFolder, fileName);
                    }
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await AnhBiaFile.CopyToAsync(fileStream);
                    }
                    truyen.AnhBia = "/images/AnhBia/" + fileName;
                }

                truyen.NgayDang = DateTime.Now;
                truyen.NgayCapNhat = DateTime.Now;
                truyen.SoChuong = 0;
                truyen.TongLuotXem = 0;
                truyen.IsDeleted = false;

                _context.Add(truyen);
                await _context.SaveChangesAsync();
                if (selectedTheLoai != null)
                {
                    foreach (var id in selectedTheLoai)
                    {
                        var theLoai = await _context.TheLoais.FindAsync(id);
                        if (theLoai != null)
                        {
                            truyen.MaTheLoais.Add(theLoai);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                    return RedirectToAction(nameof(Index));
            }

            ViewData["NguoiDang"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", truyen.NguoiDang);
            ViewData["TheLoai"] = _context.TheLoais.Where(x => x.IsDeleted == false).ToList();
            return View(truyen);
        }

        // GET: Admin/Truyens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var truyen = await _context.Truyens
                .Include(t => t.MaTheLoais)
                .FirstOrDefaultAsync(m => m.MaTruyen == id);
            if (truyen == null) return NotFound();
            ViewData["NguoiDang"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", truyen.NguoiDang);
            ViewData["TheLoai"] = _context.TheLoais.Where(x => x.IsDeleted == false).ToList();
            ViewData["SelectedTheLoai"] = truyen.MaTheLoais.Select(tl => tl.MaTheLoai).ToList();

            return View(truyen);
        }

        // POST: Admin/Truyens/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Truyen truyen, IFormFile AnhBiaFile, int[] selectedTheLoai)
        {
            if (id != truyen.MaTruyen) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Tải dữ liệu từ DB bao gồm cả các Thể loại hiện có
                    var truyenToUpdate = await _context.Truyens
                        .Include(t => t.MaTheLoais)
                        .FirstOrDefaultAsync(t => t.MaTruyen == id);

                    if (truyenToUpdate == null) return NotFound();

                    // 1. Cập nhật thông tin cơ bản
                    truyenToUpdate.TenTruyen = truyen.TenTruyen;
                    truyenToUpdate.MoTa = truyen.MoTa;
                    truyenToUpdate.TacGia = truyen.TacGia;
                    truyenToUpdate.NguoiDang = truyen.NguoiDang;
                    truyenToUpdate.LoaiTruyen = truyen.LoaiTruyen;
                    truyenToUpdate.TrangThai = truyen.TrangThai;
                    truyenToUpdate.NgayCapNhat = DateTime.Now;

                    // 2. Xử lý Ảnh bìa
                    if (AnhBiaFile != null)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "AnhBia");
                        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                        if (!string.IsNullOrEmpty(truyenToUpdate.AnhBia))
                        {
                            string oldPath = Path.Combine(_webHostEnvironment.WebRootPath, truyenToUpdate.AnhBia.TrimStart('/'));

                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        string fileName = AnhBiaFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, fileName);

                        if (System.IO.File.Exists(filePath))
                        {
                            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                            string extension = Path.GetExtension(fileName);
                            fileName = $"{fileNameWithoutExtension}_{DateTime.Now.Ticks}{extension}";
                            filePath = Path.Combine(uploadsFolder, fileName);
                        }

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await AnhBiaFile.CopyToAsync(fileStream);
                        }
                        truyenToUpdate.AnhBia = "/images/AnhBia/" + fileName;
                    }
                    truyenToUpdate.MaTheLoais.Clear();
                    if (selectedTheLoai != null)
                    {
                        foreach (var tlId in selectedTheLoai)
                        {
                            var theLoai = await _context.TheLoais.FindAsync(tlId);
                            if (theLoai != null) truyenToUpdate.MaTheLoais.Add(theLoai);
                        }
                    }

                    _context.Update(truyenToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TruyenExists(truyen.MaTruyen)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["NguoiDang"] = new SelectList(_context.ThanhViens, "TaiKhoan", "TaiKhoan", truyen.NguoiDang);
            ViewData["TheLoai"] = _context.TheLoais.Where(x => x.IsDeleted == false).ToList();
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
                truyen.IsDeleted = true;
                _context.Update(truyen);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TruyenExists(int id)
        {
            return _context.Truyens.Any(e => e.MaTruyen == id);
        }
    }
}
