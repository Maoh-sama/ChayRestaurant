namespace Cuahangchay.Models
{
    public class HoaDon
    {
        public int HoaDonID { get; set; }
        public DateTime NgayLap { get; set; } = DateTime.Now;
        public int BanID { get; set; }
        public int NhanVienID { get; set; }
        public decimal TongTien { get; set; }

        public Ban Ban { get; set; }
        public NhanVien NhanVien { get; set; }
        public List<ChiTietHoaDon> ChiTietHoaDons { get; set; }
    }
}
