using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cuahangchay.Models
{
    public class MonChay
    {
        [Key]
        public int MonID { get; set; }
        public string TenMon { get; set; }
        public string MoTa { get; set; }
        public decimal Gia { get; set; }
        public string HinhAnh { get; set; }
        public bool ConTon { get; set; }
    }

}
