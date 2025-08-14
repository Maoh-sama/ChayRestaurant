using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cuahangchay.Models
{
    [Table("KhachHang")] // Dòng này chỉ định tên bảng trong cơ sở dữ liệu
    public class KhachHang
    {
        [Key]
        public int KHID { get; set; }

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        public string? TenKH { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }
    }
}
