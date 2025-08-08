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
        public DbSet<MonChay> MonChay { get; set; }
        public DbSet<NguyenLieu> NguyenLieus { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Quan hệ ChiTietHoaDon
            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(c => c.HoaDon)
                .WithMany(h => h.ChiTietHoaDons)
                .HasForeignKey(c => c.HoaDonID);

            modelBuilder.Entity<ChiTietHoaDon>()
                .HasOne(c => c.MonChay)
                .WithMany()
                .HasForeignKey(c => c.MonID);

            // Quan hệ TaiKhoan - NhanVien
            modelBuilder.Entity<TaiKhoan>()
                .HasOne(t => t.NhanVien)
                .WithMany() // Nếu NhanVien không cần collection ngược lại
                .HasForeignKey(t => t.NhanVienID)
                .IsRequired(false); // Vì NhanVienID là nullable

            modelBuilder.Entity<HoaDon>()
                .HasOne(h => h.KhachHang)
                .WithMany()
                .HasForeignKey(h => h.KHID);

            base.OnModelCreating(modelBuilder);
        }
    }
}