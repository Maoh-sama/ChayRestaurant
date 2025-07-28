using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cuahangchay.Models
{
    public class KhachHang
    {
        [Key] // Dòng này đánh dấu đây là khóa chính
        public int KHID { get; set; }
        public string TenKH { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public int DiemTichLuy { get; set; }
    }
}
