using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cuahangchay.Models
{
    [Table("DanhGia")]
    public class DanhGia
    {
        [Key]
        public int DanhGiaID { get; set; }
        public int MonID { get; set; }
        public string Username { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime NgayDanhGia { get; set; }

        [ForeignKey("MonID")]
        public MonChay MonChay { get; set; }
    }
}