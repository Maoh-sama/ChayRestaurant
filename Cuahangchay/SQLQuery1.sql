-- ================================
-- 🍃 DATABASE: QUANLYCUAHANGCHAY
-- ================================

-- Nếu chưa có, tạo Database
-- CREATE DATABASE QuanLyCuaHangChay;
-- USE QuanLyCuaHangChay;

-- XÓA bảng cũ (nếu cần reset)
DROP TABLE IF EXISTS ChiTietHoaDon, HoaDon, MonChay, Ban, NguyenLieu, KhachHang, NhanVien, TaiKhoan, CauHinh;
GO

-- ======================================
-- E01: Món Chay
-- ======================================
CREATE TABLE MonChay (
    MonID INT PRIMARY KEY IDENTITY(1,1),
    TenMon NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255),
    Gia DECIMAL(10,2) NOT NULL,
    HinhAnh NVARCHAR(255),
    ConTon BIT DEFAULT 1
);

INSERT INTO MonChay (TenMon, MoTa, Gia, HinhAnh)
VALUES
(N'Phở chay', N'Phở với nước dùng từ rau củ và nấm', 40000, '/images/pho.jpg'),
(N'Cơm tấm chay', N'Cơm tấm với sườn chay và đồ chua', 45000, '/images/comtam.jpg'),
(N'Bún riêu chay', N'Nước dùng riêu từ đậu hủ và cà chua', 42000, '/images/bunrieu.jpg'),
(N'Gỏi cuốn chay', N'Gỏi cuốn với tàu hủ, rau sống, bún', 30000, '/images/goicuon.jpg');

-- ======================================
-- E02: Bàn
-- ======================================
CREATE TABLE Ban (
    BanID INT PRIMARY KEY IDENTITY(1,1),
    SoBan NVARCHAR(10) NOT NULL,
    TrangThai NVARCHAR(50) DEFAULT N'Trống'
);

INSERT INTO Ban (SoBan, TrangThai)
VALUES
(N'B01', N'Trống'),
(N'B02', N'Đang dùng'),
(N'B03', N'Đặt trước'),
(N'B04', N'Trống');

-- ======================================
-- E03: Nguyên liệu
-- ======================================
CREATE TABLE NguyenLieu (
    NLID INT PRIMARY KEY IDENTITY(1,1),
    TenNguyenLieu NVARCHAR(100) NOT NULL,
    SoLuong FLOAT NOT NULL,
    DonVi NVARCHAR(20),
    NgayNhap DATE DEFAULT GETDATE()
);

INSERT INTO NguyenLieu (TenNguyenLieu, SoLuong, DonVi, NgayNhap)
VALUES
(N'Nấm rơm', 10, N'kg', '2025-07-28'),
(N'Đậu hũ', 20, N'kg', '2025-07-27'),
(N'Rau sống', 15, N'kg', '2025-07-28'),
(N'Bún tươi', 30, N'kg', '2025-07-26');

-- ======================================
-- E06: Nhân viên
-- ======================================
CREATE TABLE NhanVien (
    NhanVienID INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    SDT NVARCHAR(20),
    Email NVARCHAR(100),
    VaiTro NVARCHAR(50),
    MatKhau NVARCHAR(255)
);

INSERT INTO NhanVien (HoTen, SDT, Email, VaiTro, MatKhau)
VALUES
(N'Nguyễn Quản Lý', '0901010101', 'admin@chay.vn', 'Admin', 'admin123'),
(N'Nguyễn Thu Ngân', '0902020202', 'cashier@chay.vn', 'Thu ngân', '123456'),
(N'Lê Bồi Bàn', '0903030303', 'phucvu@chay.vn', 'Phục vụ', '654321'),
(N'Trần Bếp Chính', '0904040404', 'bep@chay.vn', 'Bếp', 'bep123');

-- ======================================
-- E04: Hóa đơn và chi tiết
-- ======================================
CREATE TABLE HoaDon (
    HoaDonID INT PRIMARY KEY IDENTITY(1,1),
    NgayLap DATETIME DEFAULT GETDATE(),
    BanID INT,
    NhanVienID INT,
    TongTien DECIMAL(10,2),
    FOREIGN KEY (BanID) REFERENCES Ban(BanID),
    FOREIGN KEY (NhanVienID) REFERENCES NhanVien(NhanVienID)
);

CREATE TABLE ChiTietHoaDon (
    CTID INT PRIMARY KEY IDENTITY(1,1),
    HoaDonID INT,
    MonID INT,
    SoLuong INT NOT NULL,
    DonGia DECIMAL(10,2),
    FOREIGN KEY (HoaDonID) REFERENCES HoaDon(HoaDonID),
    FOREIGN KEY (MonID) REFERENCES MonChay(MonID)
);

-- Giả sử BanID = 1, NhanVienID = 1
INSERT INTO HoaDon (BanID, NhanVienID, TongTien)
VALUES (1, 1, 85000);

-- Giả sử HoaDonID = 1
INSERT INTO ChiTietHoaDon (HoaDonID, MonID, SoLuong, DonGia)
VALUES
(1, 1, 1, 40000),
(1, 2, 1, 45000);

-- ======================================
-- E05: Khách hàng
-- ======================================
CREATE TABLE KhachHang (
    KHID INT PRIMARY KEY IDENTITY(1,1),
    TenKH NVARCHAR(100),
    SoDienThoai NVARCHAR(20),
    Email NVARCHAR(100),
    DiemTichLuy INT DEFAULT 0
);

INSERT INTO KhachHang (TenKH, SoDienThoai, Email, DiemTichLuy)
VALUES
(N'Nguyễn Thị A', '0912345678', 'a@gmail.com', 100),
(N'Trần Văn B', '0909123456', 'b@yahoo.com', 250),
(N'Lê Thị C', '0987654321', 'c@hotmail.com', 0),
(N'Phạm Văn D', '0933221100', 'd@example.com', 50);

-- ======================================
-- E07: Tài khoản
-- ======================================
CREATE TABLE TaiKhoan (
    Username NVARCHAR(50) PRIMARY KEY,
    MatKhau NVARCHAR(255) NOT NULL,
    Quyen NVARCHAR(50) NOT NULL,
    NhanVienID INT,
    FOREIGN KEY (NhanVienID) REFERENCES NhanVien(NhanVienID)
);

INSERT INTO TaiKhoan (Username, MatKhau, Quyen, NhanVienID)
VALUES
('admin', 'admin123', 'Admin', 1),
('cashier1', '123456', 'ThuNgan', 2),
('phucvu1', '654321', 'PhucVu', 3),
('bep1', 'bep123', 'Bep', 4);

-- ======================================
-- E08: Cấu hình hệ thống
-- ======================================
CREATE TABLE CauHinh (
    [Key] NVARCHAR(50) PRIMARY KEY,
    [Value] NVARCHAR(255)
);

INSERT INTO CauHinh ([Key], [Value])
VALUES
('TenNhaHang', N'Nhà Hàng Chay An Lạc'),
('DiaChi', N'123 Nguyễn Văn Cừ, TP.HCM'),
('GioMoCua', '07:00'),
('GioDongCua', '21:00');
