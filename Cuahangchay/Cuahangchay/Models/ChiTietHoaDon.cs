using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cuahangchay.Models
{
    [Table("ChiTietHoaDon")]
    public class ChiTietHoaDon
    {
        [Key]
        public int CTID { get; set; }
        public int HoaDonID { get; set; }
        public string TenMon { get; set; } // Chỉ là trường dữ liệu, không phải foreign key
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        [JsonIgnore]
        public HoaDon HoaDon { get; set; }
    }
}