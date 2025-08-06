using Cuahangchay.Data;
using Cuahangchay.Models;
using Cuahangchay.ViewModel;
using Cuahangchay.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cuahangchay.Extensions; // Tham chiếu đến extension method

namespace Cuahangchay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult ViewCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart); // Truyền model cart vào view
        }
        [HttpPost]
        public IActionResult UpdateCart(int monId, int soLuong)
        {
            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Tìm món trong giỏ hàng
            var cartItem = cart.FirstOrDefault(c => c.MonID == monId);
            if (cartItem != null && soLuong > 0)
            {
                cartItem.SoLuong = soLuong; // Cập nhật số lượng
                HttpContext.Session.SetObjectAsJson("Cart", cart); // Lưu lại giỏ hàng
            }

            return RedirectToAction("ViewCart"); // Quay lại trang giỏ hàng
        }
        [Authorize] // Yêu cầu đăng nhập
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            if (!cart.Any())
            {
                return RedirectToAction("ViewCart");
            }

            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.Username == User.Identity.Name);
            if (taiKhoan == null)
            {
                return RedirectToAction("Login"); // Hoặc xử lý lỗi khác
            }

            var nhanVien = taiKhoan.NhanVien; // Lấy NhanVien từ TaiKhoan
            var nhanVienId = nhanVien?.NhanVienID ?? 1; // Dùng 1 nếu không tìm thấy

            var hoaDon = new HoaDon
            {
                NgayLap = DateTime.Now,
                NhanVienID = nhanVienId,
                TongTien = cart.Sum(i => i.ThanhTien)
            };

            _context.HoaDons.Add(hoaDon);
            _context.SaveChanges(); // Lưu HoaDon để lấy HoaDonID

            var chiTietHoaDons = cart.Select(item => new ChiTietHoaDon
            {
                HoaDonID = hoaDon.HoaDonID, // Sử dụng HoaDonID đã được gán sau SaveChanges
                MonID = item.MonID,
                SoLuong = item.SoLuong,
                DonGia = item.Gia
            }).ToList();

            _context.ChiTietHoaDons.AddRange(chiTietHoaDons);
            _context.SaveChanges(); // Lưu ChiTietHoaDon

            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Success");
        }
        public IActionResult AddToCart(int monId, int soLuong)
        {
            var monChay = _context.MonChay.FirstOrDefault(m => m.MonID == monId);
            if (HttpContext?.Session == null)
            {
                return StatusCode(500, "Session is not available.");
            }
            if (monChay == null || !monChay.ConTon)
            {
                return NotFound();
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.MonID == monId);
            if (cartItem != null)
            {
                cartItem.SoLuong += soLuong;
            }
            else
            {
                cart.Add(new CartItem
                {
                    MonID = monId,
                    TenMon = monChay.TenMon,
                    Gia = monChay.Gia,
                    SoLuong = soLuong
                });
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Menu");
        }

        public IActionResult Index() => View();
        public IActionResult About() => View();
        public async Task<IActionResult> Menu() => View(await _context.MonChay.ToListAsync());
        public IActionResult Reservation() => View();
        [HttpPost]
        public IActionResult Reservation(KhachHang res)
        {
            if (ModelState.IsValid) return RedirectToAction("Success");
            return View(res);
        }
        public IActionResult Contact() => View();
        public IActionResult Success() => View();
        public IActionResult ChiTiet(int id)
        {
            var sanPham = _context.MonChay.FirstOrDefault(s => s.MonID == id);
            return sanPham == null ? NotFound() : View(sanPham);
        }

        [HttpGet]
        public IActionResult Login() => View();
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.Username == model.Username && t.MatKhau == model.Password);
                if (taiKhoan != null)
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

                    // Chuyển hướng dựa trên vai trò
                    if (taiKhoan.Quyen == "Admin")
                    {
                        return RedirectToAction("QuanLyTaiKhoan"); // Chuyển hướng admin đến trang quản trị
                    }
                    else if (taiKhoan.Quyen == "Bep")
                    {
                        return RedirectToAction("Details", "TaiKhoan", new { id = taiKhoan.Username }); // Chuyển hướng Bep đến Details của chính mình
                    }
                    else if (taiKhoan.Quyen == "ThuNgan")
                    {
                        return RedirectToAction("QuanLyHoaDon", "Home"); // Chuyển hướng ThuNgan đến Quản lý Hóa đơn
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); // User về trang chủ
                    }
                }
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register() => View();
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.TaiKhoans.FirstOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }
                var newUser = new TaiKhoan
                {
                    Username = model.Username,
                    MatKhau = model.Password, // TODO: Mã hóa mật khẩu
                    Quyen = "User"
                };
                _context.TaiKhoans.Add(newUser);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult QuanLyTaiKhoan()
        {
            return RedirectToAction("Index", "TaiKhoan");
        }

        // Quản lý Kho (khung đỏ)
        [Authorize(Roles = "Admin,Bep")]
        public async Task<IActionResult> QuanLyKho() => View(await _context.NguyenLieus.ToListAsync());

        // Chỉ Bep được xem Details của Quản lý Kho
        [Authorize(Roles = "Bep")]
        public async Task<IActionResult> QuanLyKhoDetails()
        {
            // Giả định hiển thị chi tiết Kho, cần tạo View tương ứng
            var kho = await _context.NguyenLieus.ToListAsync();
            return View(kho);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> QuanLyNhanVien() => View(await _context.NhanViens.ToListAsync());

        // Quản lý Hóa đơn (khung xanh)
        [Authorize(Roles = "Admin,ThuNgan")]
        public async Task<IActionResult> QuanLyHoaDon() => View(await _context.HoaDons.Include(h => h.NhanVien).ToListAsync());

        // Quản lý Món chay (khung xanh)
        [Authorize(Roles = "Admin,ThuNgan")]
        public async Task<IActionResult> QuanLyMonChay() => View(await _context.MonChay.ToListAsync());

        // Quản lý Khách hàng (khung xanh)
        [Authorize(Roles = "Admin,ThuNgan")]
        public async Task<IActionResult> QuanLyKhachHang() => View(await _context.KhachHangs.ToListAsync());

        public async Task<IActionResult> ThongKeNguyenLieu() => View(await _context.NguyenLieus.ToListAsync());
        public async Task<IActionResult> ThongKeBanHang() => View(await _context.HoaDons.Include(h => h.ChiTietHoaDons).ThenInclude(ct => ct.MonChay).ToListAsync());
        public async Task<IActionResult> ThongKeDoanhThu() => View(await _context.HoaDons.ToListAsync());

        public IActionResult AccessDenied() => View();
    }
}