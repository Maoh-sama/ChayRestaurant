using Microsoft.AspNetCore.Mvc;
using Cuahangchay.Data;
using Cuahangchay.Models;
using Cuahangchay.Services;

namespace Cuahangchay.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // PaymentController.cs
     
        public IActionResult CreatePaymentUrl(int hoaDonId)
        {
            // FIX 1: Dùng _context.HoaDons (có s) theo đúng ảnh cậu gửi
            var hoaDon = _context.HoaDons.FirstOrDefault(h => h.HoaDonID == hoaDonId);
            if (hoaDon == null) return NotFound();

            var vnPayConfig = _configuration.GetSection("VnPay");

            var vnpay = new VnPayLibrary();

            // FIX 2: Thêm ?? "" để trị lỗi vàng (Null Reference)
            vnpay.AddRequestData("vnp_Version", vnPayConfig["Version"] ?? "2.1.0");
            vnpay.AddRequestData("vnp_Command", vnPayConfig["Command"] ?? "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnPayConfig["TmnCode"] ?? "");

            // FIX 3: Sửa lỗi đỏ (TongTien). Vì TongTien là decimal (không null), nên bỏ đoạn "?? 0" đi
            long amount = (long)(hoaDon.TongTien * 100);
            vnpay.AddRequestData("vnp_Amount", amount.ToString());

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", vnPayConfig["CurrCode"] ?? "VND");
            vnpay.AddRequestData("vnp_IpAddr", "127.0.0.1");
            vnpay.AddRequestData("vnp_Locale", vnPayConfig["Locale"] ?? "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + hoaDon.HoaDonID);
            vnpay.AddRequestData("vnp_OrderType", "other");

            // FIX 4: Thêm Request.Scheme để tạo URL đầy đủ (https://...)
            vnpay.AddRequestData("vnp_ReturnUrl", Url.Action("PaymentCallback", "Payment", null, Request.Scheme) ?? "");
            vnpay.AddRequestData("vnp_TxnRef", hoaDon.HoaDonID.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnPayConfig["BaseUrl"] ?? "", vnPayConfig["HashSecret"] ?? "");
            return Redirect(paymentUrl);
        }

        public IActionResult PaymentCallback()
        {
            var response = _configuration.GetSection("VnPay");
            var vnpay = new VnPayLibrary();

            foreach (var (key, value) in Request.Query)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            // Lấy các tham số trả về
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_TxnRef = vnpay.GetResponseData("vnp_TxnRef");
            string vnp_SecureHash = Request.Query["vnp_SecureHash"].ToString(); // .ToString() để chắc chắn không null
            string hashSecret = response["HashSecret"] ?? "";

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, hashSecret);

            if (checkSignature)
            {
                if (vnp_ResponseCode == "00")
                {
                    // FIX 5: Dùng _context.HoaDons
                    var orderId = Convert.ToInt64(vnp_TxnRef);
                    var hoaDon = _context.HoaDons.FirstOrDefault(x => x.HoaDonID == orderId);
                    if (hoaDon != null)
                    {
                        hoaDon.TrangThai = "Đã thanh toán";
                        hoaDon.PhuongThucThanhToan = "VNPAY";
                        hoaDon.MaGiaoDich = vnpay.GetResponseData("vnp_TransactionNo");
                        _context.SaveChanges();
                    }
                    ViewBag.Message = "Thanh toán thành công!";
                    return View("Success");
                }
                else
                {
                    ViewBag.Message = "Thanh toán thất bại. Mã lỗi: " + vnp_ResponseCode;
                    return View("Error");
                }
            }
            else
            {
                ViewBag.Message = "Lỗi chữ ký bảo mật!";
                return View("Error");
            }
        }
    }
}