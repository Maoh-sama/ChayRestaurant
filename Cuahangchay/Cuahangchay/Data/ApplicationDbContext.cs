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
        public DbSet<DanhGia> DanhGias { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Quan hệ ChiTietHoaDon
            modelBuilder.Entity<ChiTietHoaDon>(entity =>
            {
                entity.ToTable("ChiTietHoaDon");
                entity.HasKey(e => e.CTID);
                entity.Property(e => e.TenMon).HasMaxLength(100); // Cấu hình nvarchar(100)
                entity.HasOne(d => d.HoaDon)
                      .WithMany(h => h.ChiTietHoaDons)
                      .HasForeignKey(d => d.HoaDonID);
            });

            modelBuilder.Entity<MonChay>(entity =>
            {
                entity.ToTable("MonChay");
                entity.HasKey(e => e.MonID);
            });

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