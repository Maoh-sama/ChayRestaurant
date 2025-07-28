namespace Cuahangchay.Models
{
    public class TaiKhoan
    {
        public string Username { get; set; }
        public string MatKhau { get; set; }
        public string Quyen { get; set; }     // Admin, NhanVien,...
        public int? NhanVienID { get; set; }

        public NhanVien NhanVien { get; set; }
    }
}
