using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cuahangchay.Models
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        [Key]
        public string Username { get; set; }
        public string MatKhau { get; set; }
        public string Quyen { get; set; }     // Admin, NhanVien,...
        public int? NhanVienID { get; set; }

        public NhanVien NhanVien { get; set; }
    }
}
