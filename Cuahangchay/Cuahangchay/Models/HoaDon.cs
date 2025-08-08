using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cuahangchay.Models
{
    [Table("HoaDon")]
    public class HoaDon
    {
        [Key]
        public int HoaDonID { get; set; }
        public DateTime NgayLap { get; set; } = DateTime.Now;
        public int NhanVienID { get; set; }
        public int KHID { get; set; } // Thêm Id khách hàng
        public decimal TongTien { get; set; }
        public string  TrangThai { get; set; } = "Chờ xác nhận"; // Thêm trạng thái, mặc định là "Chờ xác nhận"
        public NhanVien NhanVien { get; set; }
        public KhachHang KhachHang { get; set; }
        public List<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}