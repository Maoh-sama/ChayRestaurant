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
        public async Task<IActionResult> Create([Bind("HoaDonID,NgayLap,NhanVienID,TongTien")] HoaDon hoaDon)
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
            var hoaDon = _context.HoaDons.Find(id);
            if (hoaDon == null)
            {
                return NotFound();
            }
            ViewBag.NhanVienID = new SelectList(_context.NhanViens, "NhanVienID", "TenNhanVien", hoaDon.NhanVienID);
            ViewBag.KhachHangID = new SelectList(_context.KhachHangs, "KhachHangID", "TenKhachHang", hoaDon.KHID);
            ViewBag.TrangThai = new SelectList(new List<string> { "Pending", "Completed", "Cancelled" }, hoaDon.TrangThai);
            return View(hoaDon);
        }
<<<<<<< HEAD
        private bool HoaDonExists(int id)
        {
            return _context.HoaDons.Any(e => e.HoaDonID == id);
        }
=======

>>>>>>> cd2eadafd2e36726da5e866fa5eeb43b08067864
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
                    var existingHoaDon = await _context.HoaDons.AsNoTracking().FirstOrDefaultAsync(h => h.HoaDonID == id);
                    if (existingHoaDon == null)
                    {
                        return NotFound();
                    }
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
                    ModelState.AddModelError("", "Concurrency error: The record you attempted to edit was modified by another user. Please reload and try again.");
                    ViewBag.NhanVienID = new SelectList(_context.NhanViens, "NhanVienID", "TenNhanVien", hoaDon.NhanVienID);
                    ViewBag.KhachHangID = new SelectList(_context.KhachHangs, "KhachHangID", "TenKhachHang", hoaDon.KHID);
                    ViewBag.TrangThai = new SelectList(new List<string> { "Pending", "Completed", "Cancelled" }, hoaDon.TrangThai);
                    return View(hoaDon);
<<<<<<< HEAD
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error: {ex.Message}");
                    return View(hoaDon);
                }
            }
=======
                }
            }

>>>>>>> cd2eadafd2e36726da5e866fa5eeb43b08067864
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
            // Lấy hóa đơn từ database
            var hoaDon = await _context.HoaDons.FindAsync(id);

            if (hoaDon != null)
            {
                // 1. Tìm tất cả các ChiTietHoaDon liên quan
                var chiTietHoaDons = await _context.ChiTietHoaDons
                                                   .Where(ct => ct.HoaDonID == hoaDon.HoaDonID)
                                                   .ToListAsync();

                // 2. Xóa các ChiTietHoaDon đó trước
                _context.ChiTietHoaDons.RemoveRange(chiTietHoaDons);

                // 3. Sau đó, xóa HoaDon
                _context.HoaDons.Remove(hoaDon);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}