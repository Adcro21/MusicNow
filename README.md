# MusicNow - Nền tảng Nghe nhạc Trực tuyến
Ứng dụng web nghe nhạc trực tuyến chú trọng vào kiến trúc mã nguồn sạch và các mẫu thiết kế phần mềm (Design Patterns).

## 🚀 Tổng quan
MusicNow cho phép người dùng khám phá, nghe và quản lý các danh sách nhạc một cách mượt mà. Dự án nhấn mạnh vào **Kiến trúc phần mềm** thông qua việc áp dụng các mẫu thiết kế nâng cao nhằm đảm bảo mã nguồn dễ bảo trì và mở rộng.

## 🛠 Công nghệ sử dụng
- **Framework:** ASP.NET MVC / .NET
- **Kiến trúc:** Phân tầng (Layered Architecture)
- **Design Patterns:** Singleton, Factory Method, Repository Pattern
- **Frontend:** Giao diện phản hồi (Responsive) với Bootstrap & JavaScript

## ✨ Các tính năng chính
- **Stream nhạc:** Phát nhạc chất lượng cao và quản lý danh sách phát (Playlist).
- **Áp dụng Design Patterns:**
  - **Singleton:** Quản lý tập trung các cấu hình hệ thống.
  - **Factory Method:** Khởi tạo đối tượng linh hoạt cho các vai trò người dùng/tính năng khác nhau.
  - **Repository:** Tách biệt logic truy cập dữ liệu khỏi logic nghiệp vụ của ứng dụng.
- **Trải nghiệm người dùng:** Giao diện tải nhanh, tối ưu hóa quản lý tài nguyên (hình ảnh, âm thanh).

## 🔧 Hướng dẫn cài đặt
1. Sao chép repository: `git clone https://github.com/Adcro21/MusicNow.git`
2. Cập nhật chuỗi kết nối SQL Server trong file `appsettings.json`.
3. Chạy lệnh `Update-Database` trong Package Manager Console.
4. Nhấn `F5` trong Visual Studio để khởi chạy ứng dụng.
