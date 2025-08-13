using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cuahangchay.Models
{
    [Table("MonChay")]
    public class MonChay
    {
        [Key]
        public int MonID { get; set; }
        public string TenMon { get; set; }           // cột này không cho null nên giữ nguyên
        public string? MoTa { get; set; }            // sửa lại cho phép null
        public decimal Gia { get; set; }             // không null => giữ nguyên
        public string? HinhAnh { get; set; }         // sửa lại cho phép null
        public bool ConTon { get; set; }            // sửa lại cho phép null
        [NotMapped]
        public IFormFile? HinhAnhUpload { get; set; }

    }
}