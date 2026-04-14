using MUSICNOW.Core.Interfaces;
using MUSICNOW.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MUSICNOW.Web.Controllers
{
    [Authorize]
    public class ArtistController : BaseController
    {
        public ArtistController(
            IUserService userService,
            IMusicService musicService,
            IPlaylistService playlistService,
            ICategoryService categoryService,
            MUSICNOW.Infrastructure.Data.MusicNowDbContext context)
            : base(userService, musicService, playlistService, categoryService, context)
        {
        }
        // GET: Artist/Details/5 (5 là UserID của nghệ sĩ)
        [HttpGet]
        public ActionResult Details(int id)
        {
            // Lấy thông tin nghệ sĩ (UserID, Tên, Avatar)
            var artistEntity = _userService.GetUserById(id);
            if (artistEntity == null || (artistEntity.Role != "MusicCreator" && artistEntity.Role != "Admin"))
            {
                // Nếu không phải nghệ sĩ thì quay về
                return RedirectToAction("Index", "Music");
            }

            // Map thông tin (Sử dụng ProfileViewModel đã có UserID)
            var artistProfile = new ProfileViewModel
            {
                UserID = artistEntity.UserID,
                Username = artistEntity.Username,
                AvatarUrl = artistEntity.AvatarUrl,
                Role = artistEntity.Role
            };

            // Lấy danh sách nhạc của nghệ sĩ này (hàm này đã có)
            var tracks = _musicService.GetTrackViewModelsByUploader(id);

            // Gộp vào 1 model
            var model = new ArtistDetailViewModel
            {
                ArtistInfo = artistProfile,
                Tracks = tracks ?? new List<TrackViewModel>()
            };

            return View(model);
        }
    }
}