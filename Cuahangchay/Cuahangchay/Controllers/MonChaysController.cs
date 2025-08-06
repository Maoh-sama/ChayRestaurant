using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cuahangchay.Data;
using Cuahangchay.Models;

namespace Cuahangchay.Controllers
{
    public class MonChaysController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MonChaysController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MonChays
        public async Task<IActionResult> Index()
        {
            return View(await _context.MonChay.ToListAsync());
        }

        // GET: MonChays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monChay = await _context.MonChay
                .FirstOrDefaultAsync(m => m.MonID == id);
            if (monChay == null)
            {
                return NotFound();
            }

            return View(monChay);
        }
        // GET: MonChays/Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MonChay monChay, IFormFile HinhAnh)
        {
            // Xóa thuộc tính HinhAnh khỏi ModelState để tránh lỗi xác thực
            ModelState.Remove("HinhAnh");

            if (ModelState.IsValid)
            {
                if (HinhAnh != null && HinhAnh.Length > 0)
                {
                    // ... (giữ nguyên logic xử lý tệp)
                }

                _context.Add(monChay);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(monChay);
        }

        // GET: MonChays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monChay = await _context.MonChay.FindAsync(id);
            if (monChay == null)
            {
                return NotFound();
            }
            return View(monChay);
        }

        // POST: MonChays/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MonChay monChay, IFormFile? HinhAnhUpload)
        {
            if (id != monChay.MonID)
                return NotFound();

            // Xóa xác thực cũ vì HinhAnhUpload không tồn tại trong DB
            ModelState.Remove("HinhAnh");

            if (ModelState.IsValid)
            {
                try
                {
                    // Nếu người dùng chọn ảnh mới thì lưu đè ảnh mới
                    if (HinhAnhUpload != null && HinhAnhUpload.Length > 0)
                    {
                        var fileName = Path.GetFileName(HinhAnhUpload.FileName);
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await HinhAnhUpload.CopyToAsync(stream);
                        }

                        // Gán đường dẫn ảnh mới
                        monChay.HinhAnh = "/img/" + fileName;
                    }

                    _context.Update(monChay);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonChayExists(monChay.MonID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(monChay);
        }


        // GET: MonChays/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monChay = await _context.MonChay
                .FirstOrDefaultAsync(m => m.MonID == id);
            if (monChay == null)
            {
                return NotFound();
            }

            return View(monChay);
        }

        // POST: MonChays/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monChay = await _context.MonChay.FindAsync(id);
            if (monChay != null)
            {
                _context.MonChay.Remove(monChay);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MonChayExists(int id)
        {
            return _context.MonChay.Any(e => e.MonID == id);
        }
    }
}
