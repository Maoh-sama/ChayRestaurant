using System.ComponentModel.DataAnnotations;

namespace Cuahangchay.Models
{
    public class CauHinh
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
