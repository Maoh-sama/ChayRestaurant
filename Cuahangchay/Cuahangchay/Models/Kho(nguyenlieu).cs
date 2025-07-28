using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cuahangchay.Models
{
    public class Kho_nguyenlieu_
    {
        [Key] // Dòng này đánh dấu đây là khóa chính
        public int NLID { get; set; }
        public string TenNguyenLieu { get; set; }
        public double SoLuong { get; set; }
        public string DonVi { get; set; }
        public DateTime NgayNhap { get; set; }
    }
}
