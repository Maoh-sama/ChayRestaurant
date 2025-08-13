using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cuahangchay.Models
{
    public class CartItem
    {
        [Required]
        public int MonID { get; set; }
        [Required]
        [StringLength(100)]
        public string TenMon { get; set; }
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Gia { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }
        public decimal ThanhTien => Gia * SoLuong;
        public string? HinhAnh { get; set; } // Thêm để hiển thị ảnh trong giỏ hàng
    }
}
