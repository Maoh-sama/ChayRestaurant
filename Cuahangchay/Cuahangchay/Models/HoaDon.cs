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
        public int KHID { get; set; }
        public decimal? TongTien { get; set; }
        public string TrangThai { get; set; } = "Chờ xác nhận";

        public string? PhuongThucThanhToan { get; set; }
        public string? MaGiaoDich { get; set; }
        public string? Token { get; set; }

        public NhanVien? NhanVien { get; set; }
        public KhachHang? KhachHang { get; set; }
        public List<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}