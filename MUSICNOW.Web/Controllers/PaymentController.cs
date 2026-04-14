using MUSICNOW.Core.Interfaces;
using MUSICNOW.Infrastructure.Data;
using MUSICNOW.Infrastructure.Services;
using MUSICNOW.Infrastructure.Services.Strategies; // Lưu ý: Bạn cần tạo thư mục này cho các Class Strategy
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MUSICNOW.Web.Controllers
{
    [Authorize] // Bắt buộc người dùng phải đăng nhập mới được vào trang thanh toán
    public class PaymentController : BaseController
    {
        // 1. CONSTRUCTOR: Kế thừa và truyền tham số cho BaseController (DI chuẩn)
        public PaymentController(
            IUserService userService,
            IMusicService musicService,
            IPlaylistService playlistService,
            ICategoryService categoryService,
            MusicNowDbContext context)
            : base(userService, musicService, playlistService, categoryService, context)
        {
        }

        // GET: /Payment/Index?type=Premium
        // Action này dùng để hiển thị trang thông tin chuyển khoản (QR Code, nội dung...)
        [HttpGet]
        public ActionResult Index(string type)
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);
            if (user == null) return RedirectToAction("Login", "Account");

            string packageType = type?.ToLower();

            // Xác định thông tin hiển thị dựa trên gói người dùng chọn
            if (packageType == "premium")
            {
                ViewBag.PackageName = "Gói Premium (1 Tháng)";
                ViewBag.Amount = 40000;
                ViewBag.Type = "Premium";
            }
            else if (packageType == "creator")
            {
                ViewBag.PackageName = "Gói Music Creator";
                ViewBag.Amount = 100000;
                ViewBag.Type = "Creator";
            }
            else
            {
                return RedirectToAction("Index", "Music");
            }

            // Tạo nội dung chuyển khoản mẫu để người dùng quét mã
            ViewBag.PaymentContent = $"UPGRADE {user.UserID}";

            return View();
        }

        // GET: /Payment/ProcessPayment?type=Premium
        // 2. ÁP DỤNG STRATEGY PATTERN TẠI ĐÂY
        [HttpGet]
        public ActionResult ProcessPayment(string type)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return RedirectToAction("Login", "Account");

            // Khởi tạo bản đồ chiến lược (Strategy Map)
            // Trong thực tế, bạn có thể đăng ký các class này vào DI Container
            var strategies = new Dictionary<string, IUpgradeStrategy>
            {
                { "premium", new PremiumUpgradeStrategy() },
                { "creator", new CreatorUpgradeStrategy() }
            };

            bool success = false;
            string key = type?.ToLower();

            // Kiểm tra xem loại gói có nằm trong danh sách chiến lược không
            if (!string.IsNullOrEmpty(key) && strategies.ContainsKey(key))
            {
                // THỰC THI CHIẾN LƯỢC: 
                // Controller chỉ gọi lệnh 'Execute', logic chi tiết nằm trong từng Class Strategy
                success = strategies[key].Execute(userId, _userService);
            }

            // Xử lý kết quả sau khi thực thi chiến lược
            if (success)
            {
                TempData["SuccessMessage"] = "Chúc mừng! Bạn đã nâng cấp tài khoản thành công.";
            }
            else
            {
                TempData["PasswordError"] = "Có lỗi xảy ra hoặc gói nâng cấp không hợp lệ. Vui lòng thử lại.";
            }

            // Sau khi xử lý xong, quay về trang cá nhân của người dùng
            return RedirectToAction("Profile", "Account");
        }

        // Hàm hỗ trợ lấy ID người dùng hiện tại từ Email đăng nhập
        private int GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = _userService.GetUserByEmail(User.Identity.Name);
                return user?.UserID ?? 0;
            }
            return 0;
        }
    }
}