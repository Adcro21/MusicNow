using System;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using MUSICNOW.Core.Interfaces;
using MUSICNOW.Infrastructure.Services;
using MUSICNOW.Infrastructure.Data;

namespace MUSICNOW.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // 1. Đăng ký DbContext (Để Unity biết cách tạo MusicNowDbContext)
            container.RegisterType<MusicNowDbContext>();

            // 2. Đăng ký các Service thông thường
            container.RegisterType<IMusicService, MusicService>();
            container.RegisterType<IPlaylistService, PlaylistService>();
            container.RegisterType<ICategoryService, CategoryService>();

            // 3. ĐĂNG KÝ USER SERVICE KÈM THEO OBSERVER PATTERN (Code mới cập nhật)
            // Sử dụng RegisterFactory theo chuẩn Unity phiên bản mới nhất
            container.RegisterFactory<IUserService>(c =>
            {
                // Lấy ra instance của DbContext từ container
                var dbContext = c.Resolve<MusicNowDbContext>();

                // Khởi tạo UserService
                var userService = new UserService(dbContext);

                // === THIẾT LẬP OBSERVER (NGƯỜI QUAN SÁT) ===
                // Đăng ký lắng nghe sự kiện OnUserActionOccurred từ UserService
                userService.OnUserActionOccurred += (userId, actionMsg) =>
                {
                    var newLog = new MUSICNOW.Core.Entities.Logs
                    {
                        UserID = userId,
                        Action = actionMsg,
                        Timestamp = DateTime.Now
                    };

                    // Thực hiện ghi log xuống DB một cách độc lập
                    dbContext.Logs.Add(newLog);
                    dbContext.SaveChanges();
                };

                // Trả về UserService đã được gắn bộ lắng nghe
                return userService;
            });

            // Thiết lập Unity làm bộ giải quyết phụ thuộc mặc định cho MVC
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}