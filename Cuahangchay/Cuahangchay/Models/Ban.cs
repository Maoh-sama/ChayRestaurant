using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Cuahangchay.Models
{
    public class Ban
    {
        [Key] // Dòng này đánh dấu đây là khóa chínhq
        public int BanID { get; set; }
        public string SoBan { get; set; }
        public string TrangThai { get; set; }
    }
}
