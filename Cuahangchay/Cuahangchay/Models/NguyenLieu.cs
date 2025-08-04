using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cuahangchay.Models
{
    [Table("NguyenLieu")] // Dòng này chỉ định tên bảng trong cơ sở dữ liệu
    public class NguyenLieu
    {
        [Key] // Dòng này đánh dấu đây là khóa chính
        public int NLID { get; set; }
        public string TenNguyenLieu { get; set; }
        public double SoLuong { get; set; }
        public string DonVi { get; set; }
        public DateTime NgayNhap { get; set; }
    }
}
