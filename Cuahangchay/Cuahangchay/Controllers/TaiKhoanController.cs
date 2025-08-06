using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cuahangchay.Data;
using Cuahangchay.Models;

namespace Cuahangchay.Controllers
{
    [Authorize]
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TaiKhoans.Include(t => t.NhanVien);
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin,Bep")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var taiKhoan = await _context.TaiKhoans.Include(t => t.NhanVien).FirstOrDefaultAsync(m => m.Username == id);
            if (taiKhoan == null) return NotFound();
            return View(taiKhoan);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["NhanVienID"] = new SelectList(_context.NhanViens, "NhanVienID", "TenNhanVien");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Username,MatKhau,Quyen,NhanVienID")] TaiKhoan taiKhoan)
        {
            if (ModelState.IsValid)
            {
                if (_context.TaiKhoans.Any(u => u.Username == taiKhoan.Username))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                    ViewData["NhanVienID"] = new SelectList(_context.NhanViens, "NhanVienID", "TenNhanVien", taiKhoan.NhanVienID);
                    return View(taiKhoan);
                }
                _context.Add(taiKhoan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NhanVienID"] = new SelectList(_context.NhanViens, "NhanVienID", "TenNhanVien", taiKhoan.NhanVienID);
            return View(taiKhoan);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null) return NotFound();
            ViewData["NhanVienID"] = new SelectList(_context.NhanViens, "NhanVienID", "TenNhanVien", taiKhoan.NhanVienID);
            return View(taiKhoan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, [Bind("Username,MatKhau,Quyen,NhanVienID")] TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.Username) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var existingTaiKhoan = await _context.TaiKhoans.FirstOrDefaultAsync(t => t.Username == id);
                    if (existingTaiKhoan == null) return NotFound();

                    if (!string.IsNullOrEmpty(taiKhoan.MatKhau))
                    {
                        existingTaiKhoan.MatKhau = taiKhoan.MatKhau;
                    }
                    existingTaiKhoan.Quyen = taiKhoan.Quyen;
                    existingTaiKhoan.NhanVienID = taiKhoan.NhanVienID;

                    _context.Update(existingTaiKhoan);
                    await _context.SaveChangesAsync();

                    if (User.Identity.Name == taiKhoan.Username)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, taiKhoan.Username),
                            new Claim(ClaimTypes.Role, taiKhoan.Quyen)
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                        };
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiKhoanExists(taiKhoan.Username)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["NhanVienID"] = new SelectList(_context.NhanViens, "NhanVienID", "TenNhanVien", taiKhoan.NhanVienID);
            return View(taiKhoan);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var taiKhoan = await _context.TaiKhoans.Include(t => t.NhanVien).FirstOrDefaultAsync(m => m.Username == id);
            if (taiKhoan == null) return NotFound();
            return View(taiKhoan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan != null)
            {
                _context.TaiKhoans.Remove(taiKhoan);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TaiKhoanExists(string id)
        {
            return _context.TaiKhoans.Any(e => e.Username == id);
        }
    }
}