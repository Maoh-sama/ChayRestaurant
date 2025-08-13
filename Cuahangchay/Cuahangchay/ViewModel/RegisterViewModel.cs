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

            [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp")]
            public string ConfirmPassword { get; set; }
      

   
    }
}