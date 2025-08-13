using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cuahangchay.Models
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        [Required]
        public string Username { get; set; }

        [Required]
        public string MatKhau { get; set; }

        [Required]
        [RegularExpression("^(Admin|Bep|ThuNgan|User)$", ErrorMessage = "Quyền phải là Admin, Bep, ThuNgan hoặc User")]
        public string Quyen { get; set; }

        public int? NhanVienID { get; set; }

        public NhanVien NhanVien { get; set; }
    }
}