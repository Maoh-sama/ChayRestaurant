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

<<<<<<< HEAD
            var khachHang = _context.KhachHangs.FirstOrDefault(k => k.TenKH == taiKhoan.Username);
            if (khachHang == null)
            {
                return RedirectToAction("UpdateKhachHang");
=======
            var khachHang = _context.KhachHangs.FirstOrDefault(k => k.TenKH == taiKhoan.Username); // Giả định dựa trên Username
            if (khachHang == null)
            {
                return RedirectToAction("UpdateKhachHang"); // Yêu cầu bổ sung thông tin KhachHang
>>>>>>> cd2eadafd2e36726da5e866fa5eeb43b08067864
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

<<<<<<< HEAD
                _context.HoaDons.Add(hoaDon);
                _context.SaveChanges();
=======
            _context.HoaDons.Add(hoaDon);
            _context.SaveChanges(); // Lưu tạm hóa đơn

>>>>>>> cd2eadafd2e36726da5e866fa5eeb43b08067864
            var chiTietHoaDons = cart.Select(item => new ChiTietHoaDon
            {
                HoaDonID = hoaDon.HoaDonID,
                MonID = item.MonID,
                SoLuong = item.SoLuong,
<<<<<<< HEAD
                DonGia = item.Gia
            }).ToList();

            _context.ChiTietHoaDons.AddRange(chiTietHoaDons);
=======
                DonGia = item.Gia   // Handle null Gia
            }).ToList();

            _context.ChiTietHoaDons.AddRange(chiTietHoaDons);
            // Không SaveChanges ngay, chờ xác nhận từ ThuNgan

            // Lưu tạm chi tiết hóa đơn và chuyển đến trang chờ xác nhận
>>>>>>> cd2eadafd2e36726da5e866fa5eeb43b08067864
            TempData["HoaDonID"] = hoaDon.HoaDonID;
            HttpContext.Session.SetObjectAsJson("PendingChiTietHoaDons", chiTietHoaDons);

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
            return View("RemoveFromCart", cart); // Hiển thị RemoveFromCart.cshtml thay vì chuyển hướng
        }
        public async Task<IActionResult> Index1() => View(await _context.MonChay.ToListAsync());
        public async Task<IActionResult> Index() => View(await _context.MonChay.ToListAsync());
        public IActionResult About() => View();
        public IActionResult About1() => View();

        public async Task<IActionResult> Menu() => View(await _context.MonChay.ToListAsync());
<<<<<<< HEAD


=======
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
>>>>>>> cd2eadafd2e36726da5e866fa5eeb43b08067864
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
            catch (Exception ex)
            {
                // Ghi log lỗi (bạn có thể thêm logging như Serilog)
                // Ví dụ: Console.WriteLine(ex.Message);
                return RedirectToAction("Error"); // Hoặc trang lỗi tùy chỉnh
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

                    // Chuyển hướng dựa trên vai trò
                    if (taiKhoan.Quyen == "Admin")
                    {
                        return RedirectToAction("QuanLyTaiKhoan"); // Chuyển hướng admin đến trang quản trị
                    }
                    else if (taiKhoan.Quyen == "Bep")
                    {
                        return RedirectToAction("Details", "Kho", new { id = taiKhoan.Username }); // Chuyển hướng Bep đến Details của chính mình
                    }
                    else if (taiKhoan.Quyen == "ThuNgan")
                    {
                        return RedirectToAction("QuanLyHoaDon", "Home"); // Chuyển hướng ThuNgan đến Quản lý Hóa đơn
                    }
                    else
                    {
                        return RedirectToAction("Index1", "Home"); // User về trang chủ
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
<<<<<<< HEAD
                try
                {
                    _context.TaiKhoans.Add(newUser);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi khi lưu tài khoản: {ex.Message}");
                    return View(model);
                }
=======
                _context.SaveChanges();
>>>>>>> cd2eadafd2e36726da5e866fa5eeb43b08067864
                return RedirectToAction("Login");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult UpdateKhachHang()
        {
            var taiKhoan = _context.TaiKhoans.FirstOrDefault(t => t.Username == User.Identity.Name);
            if (taiKhoan == null)
            {
                return RedirectToAction("Login");
            }

            var khachHang = _context.KhachHangs.FirstOrDefault(k => k.TenKH == taiKhoan.Username);
<<<<<<< HEAD
            //if (khachHang == null)
            //{
            //    khachHang = new KhachHang { TenKH = taiKhoan.Username, DiemTichLuy = 0 };
            //}
=======
            if (khachHang == null)
            {
                khachHang = new KhachHang { TenKH = taiKhoan.Username, DiemTichLuy = 0 };
            }
>>>>>>> cd2eadafd2e36726da5e866fa5eeb43b08067864
            return View(khachHang);
        }

        [HttpPost]
        public IActionResult UpdateKhachHang(KhachHang model)
        {
            if (ModelState.IsValid)
            {
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
<<<<<<< HEAD
                    _context.SaveChanges(); // Save to generate KHID
                }

                // Update the existing or newly created khachHang with model data
                khachHang.SoDienThoai = model.SoDienThoai;
                khachHang.Email = model.Email;


                _context.SaveChanges(); // Save the updated data
=======
                }

                khachHang.SoDienThoai = model.SoDienThoai;
                khachHang.Email = model.Email;
                khachHang.DiemTichLuy = model.DiemTichLuy;

                _context.SaveChanges();
>>>>>>> cd2eadafd2e36726da5e866fa5eeb43b08067864
                return RedirectToAction("Checkout");
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
        public async Task<IActionResult> QuanLyHoaDon()
        {
            var hoaDons = await _context.HoaDons
                .Include(h => h.ChiTietHoaDons)
                .Where(h => h.NhanVien != null && h.KhachHang != null) // Loại bỏ bản ghi NULL
                .ToListAsync();
            ViewBag.TrangThai = new List<SelectListItem>
            {
                new SelectListItem { Text = "Chờ xác nhận", Value = "Chờ xác nhận" },
                new SelectListItem { Text = "Đã xác nhận", Value = "Đã xác nhận" }
            };
            return View(hoaDons);
        }

        // Quản lý Món chay (khung xanh)
        [Authorize(Roles = "Admin,ThuNgan")]
        public async Task<IActionResult> QuanLyMonChay() => View(await _context.MonChay.ToListAsync());

        // Quản lý Khách hàng (khung xanh)
        [Authorize(Roles = "Admin,ThuNgan")]
        public async Task<IActionResult> QuanLyKhachHang() => View(await _context.KhachHangs.ToListAsync());

<<<<<<< HEAD
        [Authorize(Roles = "Admin,ThuNgan")]
        public async Task<IActionResult> SearchMonChay(string searchString)
        {
            var monChayQuery = _context.MonChay.AsQueryable();

            // Lưu giá trị tìm kiếm để giữ trong input khi load lại trang
            ViewData["CurrentFilter"] = searchString;

            if (!string.IsNullOrEmpty(searchString))
            {
                // Tìm kiếm theo tên món, không phân biệt hoa thường
                monChayQuery = monChayQuery.Where(m => m.TenMon.ToLower().Contains(searchString.ToLower()));
            }

            var monChayList = await monChayQuery.ToListAsync();
            return View(monChayList);
        }

=======
>>>>>>> cd2eadafd2e36726da5e866fa5eeb43b08067864
        public async Task<IActionResult> ThongKeNguyenLieu() => View(await _context.NguyenLieus.ToListAsync());
        public async Task<IActionResult> ThongKeBanHang() => View(await _context.HoaDons.Include(h => h.ChiTietHoaDons).ThenInclude(ct => ct.MonChay).ToListAsync());
        public async Task<IActionResult> ThongKeDoanhThu() => View(await _context.HoaDons.ToListAsync());

        public IActionResult AccessDenied() => View();
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}