using System.ComponentModel.DataAnnotations;

namespace Cuahangchay.ViewModels
{
    public class RegisterViewModel
    {
       
       
            [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

<<<<<<< HEAD
            [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp")]
            public string ConfirmPassword { get; set; }
      

   
=======
        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng")]
        [Display(Name = "Tên khách hàng")]
        public string TenKH { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Điểm tích lũy")]
        public int DiemTichLuy { get; set; }
>>>>>>> cd2eadafd2e36726da5e866fa5eeb43b08067864
    }
}