using Cuahangchay.Data;
using Cuahangchay.Extensions; // Tham chiếu đến extension method
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
using Microsoft.AspNetCore.Mvc.Rendering;

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
            return View(cart);
        }

        [HttpPost]
        public IActionResult UpdateCart(int monId, int soLuong)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.MonID == monId);
            if (cartItem != null && soLuong > 0)
            {
                cartItem.SoLuong = soLuong;
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("ViewCart");
        }

        [Authorize]
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
                return RedirectToAction("Login");
            }

            var khachHang = _context.KhachHangs.FirstOrDefault(k => k.TenKH == taiKhoan.Username);
            if (khachHang == null)
            {
                return RedirectToAction("UpdateKhachHang");
            }

            var nhanVien = taiKhoan.NhanVien;
            var nhanVienId = nhanVien?.NhanVienID ?? 1;

            var hoaDon = new HoaDon
            {
                NgayLap = DateTime.Now,
                NhanVienID = nhanVienId,
                KHID = khachHang.KHID,
                TongTien = cart.Sum(i => i.ThanhTien),
                TrangThai = "Chờ xác nhận"
            };

            _context.HoaDons.Add(hoaDon);
            _context.SaveChanges();

            var chiTietHoaDons = cart.Select(item => new ChiTietHoaDon
            {
                HoaDonID = hoaDon.HoaDonID,
                MonID = item.MonID,
                SoLuong = item.SoLuong,
                DonGia = item.Gia 
            }).ToList();

            TempData["HoaDonID"] = hoaDon.HoaDonID;
            HttpContext.Session.SetObjectAsJson("PendingChiTietHoaDons", chiTietHoaDons);
            // Clear the cart after successful checkout
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Success");
        }

        [Authorize(Roles = "ThuNgan")]
        public IActionResult ConfirmCheckout(int hoaDonId)
        {
            var hoaDon = _context.HoaDons.FirstOrDefault(h => h.HoaDonID == hoaDonId);
            if (hoaDon == null || hoaDon.TrangThai != "Chờ xác nhận")
            {
                return NotFound();
            }

            var chiTietHoaDons = HttpContext.Session.GetObjectFromJson<List<ChiTietHoaDon>>("PendingChiTietHoaDons");
            if (chiTietHoaDons != null)
            {
                _context.ChiTietHoaDons.AddRange(chiTietHoaDons);
                hoaDon.TrangThai = "Đã xác nhận";
                _context.SaveChanges();
                HttpContext.Session.Remove("PendingChiTietHoaDons");
                HttpContext.Session.Remove("Cart");
            }

            return RedirectToAction("QuanLyHoaDon");
        }

        public IActionResult PendingCheckout()
        {
            var hoaDonId = TempData["HoaDonID"] as int?;
            if (!hoaDonId.HasValue)
            {
                return RedirectToAction("Index");
            }
            var hoaDon = _context.HoaDons
                .Include(h => h.KhachHang)
                .Include(h => h.NhanVien)
                .Include(h => h.ChiTietHoaDons)
                .ThenInclude(ct => ct.MonChay)
                .FirstOrDefault(h => h.HoaDonID == hoaDonId);
            if (hoaDon == null || hoaDon.ChiTietHoaDons == null || hoaDon.KhachHang == null || hoaDon.NhanVien == null)
            {
                return NotFound();
            }
            return View(hoaDon);
        }




//////AddToCart///////



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

        [HttpPost]
        public IActionResult RemoveFromCart(int monId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var cartItem = cart.FirstOrDefault(c => c.MonID == monId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return View("RemoveFromCart", cart);
        }

        public async Task<IActionResult> Index() => View(await _context.MonChay.ToListAsync());
        public async Task<IActionResult> Index1() => View(await _context.MonChay.ToListAsync());

        public IActionResult About() => View();

        public async Task<IActionResult> Menu() => View(await _context.MonChay.ToListAsync());

        [HttpGet]
        public async Task<IActionResult> SearchMenu(string query)
        {
            var monChayList = await _context.MonChay.ToListAsync();
            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToLower();
                monChayList = monChayList
                    .Where(m => m.TenMon.ToLower().Contains(query))
                    .ToList();
            }
            return PartialView("_MenuPartial", monChayList);
        }

        [HttpPost]
        public IActionResult SubmitRating(int monId, int rating, string comment)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var danhGia = new DanhGia
            {
                MonID = monId,
                Username = User.Identity.Name,
                Rating = rating,
                Comment = comment,
                NgayDanhGia = DateTime.Now
            };

            try
            {
                _context.DanhGias.Add(danhGia);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                return RedirectToAction("Error");
            }

            return RedirectToAction("ChiTiet", new { id = monId });
        }

        public IActionResult Reservation() => View();

        [HttpPost]
        public IActionResult Reservation(KhachHang res)
        {
            if (ModelState.IsValid) return RedirectToAction("Success");
            return View(res);
        }

        public IActionResult Contact() => View();
        public IActionResult Contact1() => View();
        public IActionResult About1() => View();
        public IActionResult Success() => View();

        public IActionResult ChiTiet(int id)
        {
            var monChay = _context.MonChay.FirstOrDefault(m => m.MonID == id);
            if (monChay == null)
            {
                return NotFound();
            }

            var ratings = _context.DanhGias
                                 .Where(d => d.MonID == id)
                                 .ToList() ?? new List<DanhGia>();

            var viewModel = new MonChayViewModel
            {
                MonChay = monChay,
                Ratings = ratings
            };

            return View(viewModel);
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

                    if (taiKhoan.Quyen == "Admin")
                    {
                        return RedirectToAction("QuanLyTaiKhoan");
                    }
                    else if (taiKhoan.Quyen == "Bep")
                    {
                        return RedirectToAction("Details", "Kho", new { id = taiKhoan.Username });
                    }
                    else if (taiKhoan.Quyen == "ThuNgan")
                    {
                        return RedirectToAction("QuanLyHoaDon", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index1", "Home");
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
                // Check if the username already exists
                var existingUser = _context.TaiKhoans.FirstOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                // Create new TaiKhoan
                var newUser = new TaiKhoan
                {
                    Username = model.Username,
                    MatKhau = model.Password, // TODO: Mã hóa mật khẩu
                    Quyen = "User"
                };

                // Create new KhachHang
                var newKhachHang = new KhachHang
                {
                    TenKH = model.TenKH, // Use TenKH from RegisterViewModel
                    SoDienThoai = model.SoDienThoai,
                    Email = model.Email,
                    
                };

                try
                {
                    // Add TaiKhoan and KhachHang to the database
                    _context.TaiKhoans.Add(newUser);
                    _context.KhachHangs.Add(newKhachHang);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi lưu dữ liệu: {ex.Message}");
                    return View(model);
                }

                return RedirectToAction("Login");
            }

            return View(model);
        }


        /// /////////Cap Nhat Khach Hang//////////



        [HttpGet]
        public IActionResult UpdateKhachHang()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }
            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.Username == User.Identity.Name);
            if (taiKhoan == null)
            {
                return RedirectToAction("Login");
            }

            var khachHang = _context.KhachHangs.FirstOrDefault(k => k.TenKH == taiKhoan.Username);
            return View(khachHang);
        }

        [HttpPost]
        public IActionResult UpdateKhachHang(KhachHang model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine(string.Join("; ", errors)); // Ghi log lỗi
                return View(model);
            }

            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.Username == User.Identity.Name);
            if (taiKhoan == null)
            {
                return RedirectToAction("Login");
            }

            var khachHang = _context.KhachHangs.FirstOrDefault(k => k.TenKH == taiKhoan.Username);
            if (khachHang == null)
            {
                khachHang = new KhachHang { TenKH = taiKhoan.Username };
                _context.KhachHangs.Add(khachHang);
            }

            // Gán TenKH từ taiKhoan.Username và cập nhật các trường khác
            khachHang.TenKH = taiKhoan.Username;
            khachHang.SoDienThoai = model.SoDienThoai;
            khachHang.Email = model.Email;

            try
            {
                _context.Update(khachHang); // Đảm bảo theo dõi đối tượng
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // Ghi log lỗi
                ModelState.AddModelError("", $"Lỗi khi lưu dữ liệu: {ex.Message}");
                return View(model);
            }

            return RedirectToAction("Checkout");
        }


        /// ///// Quản lý tài khoản


        [Authorize(Roles = "Admin")]
        public IActionResult QuanLyTaiKhoan()
        {
            return RedirectToAction("Index", "TaiKhoan");
        }

        [Authorize(Roles = "Admin,Bep")]
        public async Task<IActionResult> QuanLyKho() => View(await _context.NguyenLieus.ToListAsync());

        [Authorize(Roles = "Bep")]
        public async Task<IActionResult> QuanLyKhoDetails()
        {
            var kho = await _context.NguyenLieus.ToListAsync();
            return View(kho);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> QuanLyNhanVien() => View(await _context.NhanViens.ToListAsync());

        [Authorize(Roles = "Admin,ThuNgan")]
        public async Task<IActionResult> QuanLyHoaDon()
        {
            var hoaDons = await _context.HoaDons
                .Include(h => h.ChiTietHoaDons)
                .Where(h => h.NhanVien != null && h.KhachHang != null)
                .ToListAsync();
            ViewBag.TrangThai = new List<SelectListItem>
        {
            new SelectListItem { Text = "Chờ xác nhận", Value = "Chờ xác nhận" },
            new SelectListItem { Text = "Đã xác nhận", Value = "Đã xác nhận" }
        };
            return View(hoaDons);
        }

        [Authorize(Roles = "Admin,ThuNgan")]
        public async Task<IActionResult> QuanLyMonChay() => View(await _context.MonChay.ToListAsync());

        [Authorize(Roles = "Admin,ThuNgan")]
        public async Task<IActionResult> QuanLyKhachHang() => View(await _context.KhachHangs.ToListAsync());

        public async Task<IActionResult> ThongKeNguyenLieu() => View(await _context.NguyenLieus.ToListAsync());
        public async Task<IActionResult> ThongKeBanHang() => View(await _context.HoaDons.Include(h => h.ChiTietHoaDons).ThenInclude(ct => ct.MonChay).ToListAsync());
        public async Task<IActionResult> ThongKeDoanhThu(DateTime? startDate, DateTime? endDate)
        {
            var hoaDons = await _context.HoaDons
                .Include(h => h.KhachHang)
                .Include(h => h.NhanVien)
                .Include(h => h.ChiTietHoaDons)
                .ThenInclude(ct => ct.MonChay)
                .ToListAsync();

            // Lọc hóa đơn theo ngày nếu có tham số
            if (startDate.HasValue)
            {
                hoaDons = hoaDons.Where(h => h.NgayLap.Date >= startDate.Value.Date).ToList();
            }
            if (endDate.HasValue)
            {
                hoaDons = hoaDons.Where(h => h.NgayLap.Date <= endDate.Value.Date).ToList();
            }

            // Thống kê tổng doanh thu theo ngày
            var dailyRevenue = hoaDons
                .GroupBy(h => h.NgayLap.Date)
                .Select(g => new
                {
                    Ngay = g.Key,
                    TongTien = g.Sum(h => h.TongTien)
                })
                .OrderBy(x => x.Ngay)
                .ToList();

            ViewBag.DailyRevenue = dailyRevenue;
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            // Thống kê số lượng hóa đơn theo khoảng giá
            var thongKeKhoangGia = hoaDons
                .GroupBy(h => h.TongTien switch
                {
                    var t when t < 500000 => "Dưới 500k",
                    var t when t >= 500000 && t <= 2000000 => "500k - 2 triệu",
                    _ => "Trên 2 triệu"
                })
                .Select(g => new
                {
                    KhoangGia = g.Key,
                    SoLuong = g.Count()
                })
                .ToList();

            ViewBag.ThongKeKhoangGia = thongKeKhoangGia;

            // Trả về model là danh sách các hóa đơn đã được lọc
            return View(hoaDons);
        }
        public IActionResult AccessDenied() => View();

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
