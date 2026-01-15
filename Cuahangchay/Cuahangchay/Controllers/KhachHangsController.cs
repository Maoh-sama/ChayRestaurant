using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cuahangchay.Data;
using Cuahangchay.Models;

namespace Cuahangchay.Controllers
{
    public class KhachHangsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhachHangsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: KhachHangs
        public async Task<IActionResult> Index()
        {
            return View(await _context.KhachHangs.ToListAsync());
        }

        // GET: KhachHangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.KHID == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        // GET: KhachHangs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KhachHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KHID,SoDienThoai,Email,DiemTichLuy")] KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                _context.Add(khachHang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(khachHang);
        }

        // GET: KhachHangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            return View(khachHang);
        }

        // POST: KhachHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KHID,SoDienThoai,Email,DiemTichLuy")] KhachHang khachHang)
        {
            if (id != khachHang.KHID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(khachHang);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KhachHangExists(khachHang.KHID))
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
            return View(khachHang);
        }

        // GET: KhachHangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(m => m.KHID == id);
            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        // POST: KhachHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang != null)
            {
                // Xóa các bản ghi HoaDon liên quan
                var hoaDons = _context.HoaDons.Where(hd => hd.KHID == id).ToList();
                _context.HoaDons.RemoveRange(hoaDons);

                _context.KhachHangs.Remove(khachHang);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool KhachHangExists(int id)
        {
            return _context.KhachHangs.Any(e => e.KHID == id);
        }
        // Hàm xuất danh sách Email ra file Excel/CSV
        public IActionResult ExportEmails()
        {
            var danhSachKhachHang = _context.KhachHangs.ToList();

            var data = danhSachKhachHang
                       .Where(k => !string.IsNullOrEmpty(k.Email))
                       .Select(k => new
                       {
                           // FIX 1: Dùng toán tử ?? để đảm bảo nó luôn là string
                           Ten = k.TenKH ?? "Khách vãng lai",
                           Email = k.Email
                       })
                       .ToList();

            var builder = new StringBuilder();
            builder.AppendLine("HoTen,Email");

            foreach (var item in data)
            {
                // FIX 2: Thêm .ToString() cho chắc chắn, dù FIX 1 đã lo rồi nhưng "thừa hơn thiếu" để trị lỗi Replace
                string tenAnToan = item.Ten.ToString().Replace(",", " ");
                builder.AppendLine($"{tenAnToan},{item.Email}");
            }

            var preamble = Encoding.UTF8.GetPreamble();
            var body = Encoding.UTF8.GetBytes(builder.ToString());
            var finalData = new byte[preamble.Length + body.Length];

            Array.Copy(preamble, finalData, preamble.Length);
            Array.Copy(body, 0, finalData, preamble.Length, body.Length);

            return File(finalData, "text/csv", "danh-sach-email.csv");
        }
    }
}
