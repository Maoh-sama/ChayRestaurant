using Cuahangchay.Data;
using Cuahangchay.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using System.Linq; // Thêm dòng này để dùng FirstOrDefault()
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Cuahangchay.ViewModels;
using Cuahangchay.ViewModel;

namespace Cuahangchay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        public IActionResult About() => View();

        public IActionResult Menu()
        {
            try
            {
                List<MonChay> monChaylist = _context.MonChays.ToList();
                if (monChaylist == null)
                {
                    monChaylist = new List<MonChay>(); // Trả về danh sách rỗng nếu null
                }
                return View(monChaylist);
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc trả về view lỗi
                return Content("Lỗi: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Reservation() => View();

        [HttpPost]
        public IActionResult Reservation(KhachHang res)
        {
            if (ModelState.IsValid)
                return RedirectToAction("Success");
            return View(res);
        }

        public IActionResult Contact() => View();

        public IActionResult Success() => View();

        public IActionResult ChiTiet(int id)
        {
            var sanPham = _context.MonChays.FirstOrDefault(s => s.MonID == id);

            if (sanPham == null)
            {
                // Trả về trang lỗi 404 hoặc một trang thông báo
                return NotFound();
            }

            return View(sanPham);
        }

        // --- Bắt đầu phương thức Đăng nhập (Login) đã sửa ---

        // Action hiển thị form đăng nhập (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Action xử lý form đăng nhập (POST)
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Tìm tài khoản trong database dựa trên tên đăng nhập và mật khẩu
                // Lưu ý: Trong thực tế, bạn nên mã hóa mật khẩu trước khi lưu và so sánh
                var taiKhoan = _context.TaiKhoans
                    .FirstOrDefault(t => t.Username == model.Username && t.MatKhau == model.Password);

                if (taiKhoan != null)
                {
                    // 2. Nếu tìm thấy tài khoản, tạo Claims và đăng nhập
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, taiKhoan.Username),
                        new Claim(ClaimTypes.Role, taiKhoan.Quyen) // Gán quyền từ thuộc tính Quyen
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60) // Thời gian hết hạn cookie
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Đăng nhập thành công, chuyển hướng đến trang Admin
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    // Nếu không tìm thấy, thêm lỗi vào ModelState
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            // Nếu có lỗi, trả về view với model để hiển thị lại form
            return View(model);
        }
        // Dăng ki//
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tài khoản đã tồn tại chưa
                var existingUser = _context.TaiKhoans.FirstOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                // Tạo tài khoản mới
                var newUser = new TaiKhoan
                {
                    Username = model.Username,
                    MatKhau = model.Password,
                    Quyen = "user" // hoặc mặc định là "user"
                };

                _context.TaiKhoans.Add(newUser);
                _context.SaveChanges();

                // Chuyển hướng về Login sau khi đăng ký
                return RedirectToAction("Login");
            }

            return View(model);
        }

    }
}