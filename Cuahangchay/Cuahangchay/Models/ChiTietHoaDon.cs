// Đảm bảo có dòng này
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cuahangchay.Models
{
    public class ChiTietHoaDon
    {
        [Key] // Dòng này phải có
        public int CTID { get; set; }
        public int HoaDonID { get; set; }
        public int MonID { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        public MonChay MonChay { get; set; }
        public HoaDon HoaDon { get; set; }
    }
}