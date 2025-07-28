using Cuahangchay.Models;
using Microsoft.EntityFrameworkCore;  // Bắt buộc phải có

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
        public DbSet<Kho_nguyenlieu_> Kho_nguyenlieu_s { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<CauHinh> CauHinhs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình ChiTietHoaDon là một keyless entity type
            modelBuilder.Entity<ChiTietHoaDon>().HasNoKey();

            // Đừng quên gọi base.OnModelCreating nếu bạn có các cấu hình khác
            base.OnModelCreating(modelBuilder);
        }
    }
}
