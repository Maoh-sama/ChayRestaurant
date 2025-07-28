using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cuahangchay.Models
{
    public class CauHinh
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
