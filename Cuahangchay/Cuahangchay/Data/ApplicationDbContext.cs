using Microsoft.EntityFrameworkCore; // Bắt buộc phải có
using Cuahangchay.Models; // Đảm bảo bạn có namespace cho các Models của bạn
using System.ComponentModel.DataAnnotations; 
namespace Cuahangchay.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet<T> cho các bảng
        public DbSet<MonChay> MonChays { get; set; }
        public DbSet<Ban> Bans { get; set; }
        public DbSet<NguyenLieu> NguyenLieus { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<CauHinh> CauHinhs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Bỏ dòng này đi HOẶC comment nó hoàn toàn:
            // modelBuilder.Entity<ChiTietHoaDon>().HasNoKey();

            // Các mối quan hệ của bạn được định nghĩa đúng theo quy ước
            // nên EF Core có thể tự động nhận diện chúng.
            // Nếu sau này bạn gặp lỗi với các mối quan hệ phức tạp hơn,
            // bạn có thể thêm cấu hình tường minh tại đây.

            base.OnModelCreating(modelBuilder);
        }
    }
}