using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cuahangchay.Models
{
    [Table("KhachHang")] // Dòng này chỉ định tên bảng trong cơ sở dữ liệu
    public class KhachHang
    {
        [Key] // Dòng này đánh dấu đây là khóa chính
        public int KHID { get; set; }
        public string? TenKH { get; set; }  // Already likely string, but ensure ?
        public string? SoDienThoai { get; set; }
        public string? Email { get; set; }
     
    }
}
