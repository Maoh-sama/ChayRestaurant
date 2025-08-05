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
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TaiKhoan
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TaiKhoans.Include(t => t.NhanVien);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TaiKhoan/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .FirstOrDefaultAsync(m => m.Username == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
        }

        // GET: TaiKhoan/Create
        public IActionResult Create()
        {
            ViewData["NhanVienID"] = new SelectList(_context.NhanViens, "NhanVienID", "NhanVienID");
            return View();
        }

        // POST: TaiKhoan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,MatKhau,Quyen,NhanVienID")] TaiKhoan taiKhoan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taiKhoan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NhanVienID"] = new SelectList(_context.NhanViens, "NhanVienID", "NhanVienID", taiKhoan.NhanVienID);
            return View(taiKhoan);
        }

        // GET: TaiKhoan/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }
            ViewData["NhanVienID"] = new SelectList(_context.NhanViens, "NhanVienID", "NhanVienID", taiKhoan.NhanVienID);
            return View(taiKhoan);
        }

        // POST: TaiKhoan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Username,MatKhau,Quyen,NhanVienID")] TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.Username)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taiKhoan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiKhoanExists(taiKhoan.Username))
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
            ViewData["NhanVienID"] = new SelectList(_context.NhanViens, "NhanVienID", "NhanVienID", taiKhoan.NhanVienID);
            return View(taiKhoan);
        }

        // GET: TaiKhoan/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.NhanVien)
                .FirstOrDefaultAsync(m => m.Username == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
        }

        // POST: TaiKhoan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan != null)
            {
                _context.TaiKhoans.Remove(taiKhoan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaiKhoanExists(string id)
        {
            return _context.TaiKhoans.Any(e => e.Username == id);
        }
    }
}
