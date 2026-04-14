using System.Web.Mvc;
using MUSICNOW.Core.Entities;
using MUSICNOW.Core.Interfaces;
using MUSICNOW.Infrastructure.Services;
using MUSICNOW.Infrastructure.Data;
using System.Collections.Generic; // << Thêm
using System.Linq; // << Thêm

namespace MUSICNOW.Web.Controllers
{
    public class BaseController : Controller
    {
        // (Các service cũ giữ nguyên)
        protected readonly IUserService _userService;
        protected readonly IMusicService _musicService;
        protected readonly IPlaylistService _playlistService;
        protected readonly ICategoryService _categoryService;
        protected readonly MusicNowDbContext _context;

        public BaseController(
             IUserService userService,
             IMusicService musicService,
             IPlaylistService playlistService,
             ICategoryService categoryService,
             MusicNowDbContext context)
        {
            // Gán các biến nội bộ bằng chính các tham số truyền vào ở trên (chữ viết thường)
            _userService = userService;
            _musicService = musicService;
            _playlistService = playlistService;
            _categoryService = categoryService;
            _context = context;
        }

        // HÀM QUAN TRỌNG (ĐÃ SỬA)
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (User.Identity.IsAuthenticated)
            {
                var user = _userService.GetUserByEmail(User.Identity.Name);

                if (user != null)
                {
                    _userService.CheckAndUpdatePremiumStatus(user);

                    // Gán 3 biến cũ
                    ViewBag.IsUserPremium = user.IsPremium;
                    ViewData["CanUpload"] = _musicService.CanUploadMusic(user.UserID);
                    ViewData["CanCreatePlaylist"] = _playlistService.CanCreateNewPlaylist(user.UserID);

                    // === THÊM KHỐI NÀY VÀO (Bước 5) ===
                    // Lấy danh sách playlist và gửi ra ViewBag để Sidebar dùng
                    ViewBag.UserPlaylists = _playlistService.GetUserPlaylists(user.UserID);

                    ViewBag.UserRole = user.Role;
                }
            }
            else
            {
                // (Code "else" cũ giữ nguyên)
                ViewBag.IsUserPremium = false;
                ViewData["CanUpload"] = false;
                ViewData["CanCreatePlaylist"] = false;
                ViewBag.UserPlaylists = new List<Playlist>(); // Gán list rỗng

                ViewBag.UserRole = "Guest";
            }
        }
    }
}