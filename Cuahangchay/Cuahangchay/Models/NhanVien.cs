using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cuahangchay.Models
{
    public class NhanVien
    {
        [Key] // Dòng này đánh dấu đây là khóa chính
        public int NhanVienID { get; set; }
        public string HoTen { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string VaiTro { get; set; }   // Ví dụ: Thu ngân, Quản lý
        public string MatKhau { get; set; }  // Có thể dùng để xác thực tạm
    }
}
