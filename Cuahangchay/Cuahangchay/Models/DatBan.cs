using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cuahangchay.Models
{
    public class DatBan
    {
        [Key] // Dòng này đánh dấu đây là khóa chính
        public int DatBanID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng")]
        [Display(Name = "Tên Khách Hàng")]
        public string TenKhachHang { get; set; }

        [Phone]
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày giờ")]
        [Display(Name = "Ngày giờ đặt")]
        public DateTime NgayGio { get; set; }

        [Range(1, 100)]
        [Display(Name = "Số người")]
        public int SoNguoi { get; set; }

        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }
    }
}
