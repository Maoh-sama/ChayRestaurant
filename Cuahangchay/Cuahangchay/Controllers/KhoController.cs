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
    public class KhoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Kho
        public async Task<IActionResult> Index()
        {
            return View(await _context.NguyenLieus.ToListAsync());
        }

        // GET: Kho/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguyenLieu = await _context.NguyenLieus
                .FirstOrDefaultAsync(m => m.NLID == id);
            if (nguyenLieu == null)
            {
                return NotFound();
            }

            return View(nguyenLieu);
        }

        // GET: Kho/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Kho/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NLID,TenNguyenLieu,SoLuong,DonVi,NgayNhap")] NguyenLieu nguyenLieu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nguyenLieu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nguyenLieu);
        }

        // GET: Kho/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguyenLieu = await _context.NguyenLieus.FindAsync(id);
            if (nguyenLieu == null)
            {
                return NotFound();
            }
            return View(nguyenLieu);
        }

        // POST: Kho/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NLID,TenNguyenLieu,SoLuong,DonVi,NgayNhap")] NguyenLieu nguyenLieu)
        {
            if (id != nguyenLieu.NLID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nguyenLieu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguyenLieuExists(nguyenLieu.NLID))
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
            return View(nguyenLieu);
        }

        // GET: Kho/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguyenLieu = await _context.NguyenLieus
                .FirstOrDefaultAsync(m => m.NLID == id);
            if (nguyenLieu == null)
            {
                return NotFound();
            }

            return View(nguyenLieu);
        }

        // POST: Kho/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguyenLieu = await _context.NguyenLieus.FindAsync(id);
            if (nguyenLieu != null)
            {
                _context.NguyenLieus.Remove(nguyenLieu);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NguyenLieuExists(int id)
        {
            return _context.NguyenLieus.Any(e => e.NLID == id);
        }
    }
}
