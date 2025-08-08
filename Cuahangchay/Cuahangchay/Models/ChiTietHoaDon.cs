// Đảm bảo có dòng này
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // Để sử dụng JsonIgnore

namespace Cuahangchay.Models
{
    [Table("ChiTietHoaDon")] // Dòng này chỉ định tên bảng trong cơ sở dữ liệu
    public class ChiTietHoaDon
    {
        [Key] // Dòng này phải có
        public int CTID { get; set; }
        public int HoaDonID { get; set; }
        public int MonID { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        public MonChay MonChay { get; set; }
        [JsonIgnore]
        public HoaDon HoaDon { get; set; }

    }
}