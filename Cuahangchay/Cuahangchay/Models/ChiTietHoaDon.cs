namespace Cuahangchay.Models
{
    public class ChiTietHoaDon
    {
        public int CTID { get; set; }
        public int HoaDonID { get; set; }
        public int MonID { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }

        public MonChay MonChay { get; set; }
        public HoaDon HoaDon { get; set; }
    }
}
