using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cuahangchay.Models
{
    [Table("HoaDon")] // Dòng này chỉ định tên bảng trong cơ sở dữ liệu
    public class HoaDon
    {
        [Key]
        public int HoaDonID { get; set; }
        public DateTime NgayLap { get; set; } = DateTime.Now;
        public int BanID { get; set; }
        public int NhanVienID { get; set; }
        public decimal TongTien { get; set; }
        public NhanVien NhanVien { get; set; }
        public List<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}
