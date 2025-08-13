using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cuahangchay.Models
{
    [Table("Ban")]
    public class Ban
    {
        [Key]
        public int BanID { get; set; }
        public string SoBan { get; set; }
        public string TrangThai { get; set; }
    }
}
