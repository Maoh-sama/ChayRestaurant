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
    public class HoaDonController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoaDonController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HoaDon
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.HoaDons.Include(h => h.NhanVien);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: HoaDon/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.NhanVien)
                .FirstOrDefaultAsync(m => m.HoaDonID == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // GET: HoaDon/Create
        public IActionResult Create()
        {
            ViewData["NhanVienID"] = new SelectList(_context.NhanViens, "NhanVienID", "NhanVienID");
            return View();
        }

        // POST: HoaDon/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HoaDonID,NgayLap,BanID,NhanVienID,TongTien")] HoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoaDon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NhanVienID"] = new SelectList(_context.NhanViens, "NhanVienID", "NhanVienID", hoaDon.NhanVienID);
            return View(hoaDon);
        }

        // GET: HoaDon/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var hoaDon = _context.HoaDons.Find(id); // Assuming Entity Framework
            if (hoaDon == null)
            {
                return NotFound(); // Or handle the case appropriately
            }
            ViewBag.NhanVienID = new SelectList(_context.NhanViens, "NhanVienID", "TenNhanVien");
            ViewBag.TrangThai = new SelectList(new List<string> { "Pending", "Completed", "Cancelled" }); // Example statuses
            return View(hoaDon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HoaDonID,NgayLap,NhanVienID,TongTien,TrangThai")] HoaDon hoaDon)
        {
            if (id != hoaDon.HoaDonID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDon);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(hoaDon.HoaDonID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(hoaDon);
        }

        // GET: HoaDon/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.NhanVien)
                .FirstOrDefaultAsync(m => m.HoaDonID == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // POST: HoaDon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonExists(int id)
        {
            return _context.HoaDons.Any(e => e.HoaDonID == id);
        }
    }
}