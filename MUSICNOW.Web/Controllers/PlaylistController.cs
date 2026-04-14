using System.Web.Mvc;
using MUSICNOW.Core.ViewModels;
using MUSICNOW.Core.Entities;
using System.Linq;
using System.Collections.Generic;
using MUSICNOW.Core.Interfaces;

namespace MUSICNOW.Web.Controllers
{
    [Authorize]
    public class PlaylistController : BaseController
    {
        public PlaylistController(
            IUserService userService,
            IMusicService musicService,
            IPlaylistService playlistService,
            ICategoryService categoryService,
            MUSICNOW.Infrastructure.Data.MusicNowDbContext context)
            : base(userService, musicService, playlistService, categoryService, context)
        {
        }
        // (Action Create (GET/POST) giữ nguyên)
        #region Create Playlist
        [HttpGet]
        public ActionResult Create()
        {
            if (!((bool)(ViewData["CanCreatePlaylist"] ?? false)))
            {
                return RedirectToAction("Index", "Music");
            }
            ViewBag.ActivePage = "CreatePlaylist";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreatePlaylistViewModel model)
        {
            var userId = GetCurrentUserId();
            if (!((bool)(ViewData["CanCreatePlaylist"] ?? false)))
            {
                return RedirectToAction("Index", "Music");
            }
            if (ModelState.IsValid)
            {
                int newPlaylistId = _playlistService.CreatePlaylist(userId, model.Name);
                if (newPlaylistId > 0)
                {
                    return RedirectToAction("Details", new { id = newPlaylistId });
                }
                else
                {
                    ModelState.AddModelError("", "Không thể tạo playlist.");
                }
            }
            ViewBag.ActivePage = "CreatePlaylist";
            return View(model);
        }
        #endregion

        // (Action Details (GET) giữ nguyên)
        #region Details
        [HttpGet]
        public ActionResult Details(int id)
        {
            var userId = GetCurrentUserId();
            var playlist = _playlistService.GetPlaylistDetails(id, userId);

            if (playlist == null)
            {
                return RedirectToAction("Index", "Music");
            }
            var songsInPlaylist = _playlistService.GetSongsInPlaylist(id);

            ViewBag.PlaylistName = playlist.Name;
            ViewBag.PlaylistCover = playlist.CoverArtUrl;
            ViewBag.PlaylistID = playlist.PlaylistID;
            ViewBag.ActivePlaylistId = id;

            return View(songsInPlaylist);
        }
        #endregion

        // (Action Edit / Delete (GET/POST) giữ nguyên)
        #region Edit / Delete Playlist

        // GET: /Playlist/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var userId = GetCurrentUserId();
            var playlist = _playlistService.GetPlaylistDetails(id, userId);

            if (playlist == null)
            {
                return RedirectToAction("Index", "Music");
            }

            var model = new EditPlaylistViewModel
            {
                PlaylistID = playlist.PlaylistID,
                Name = playlist.Name,
                ExistingCoverArtUrl = playlist.CoverArtUrl
            };

            return View(model);
        }

        // POST: /Playlist/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditPlaylistViewModel model)
        {
            var userId = GetCurrentUserId();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool success = _playlistService.UpdatePlaylist(model, userId);

            if (success)
            {
                return RedirectToAction("Details", new { id = model.PlaylistID });
            }

            ModelState.AddModelError("", "Không thể cập nhật playlist.");
            return View(model);
        }

        // GET: /Playlist/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var userId = GetCurrentUserId();
            var playlist = _playlistService.GetPlaylistDetails(id, userId);

            if (playlist == null)
            {
                return RedirectToAction("Index", "Music");
            }

            return View(playlist);
        }

        // POST: /Playlist/Delete/5
        [HttpPost] // Xóa ActionName("Delete")
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            bool success = _playlistService.DeletePlaylist(id, userId);
            return RedirectToAction("Index", "Music");
        }

        #endregion

        // === THÊM ACTION MỚI NÀY VÀO (Bước 3) ===
        // POST: /Playlist/RemoveSongFromPlaylist?playlistId=5&musicId=10
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public JsonResult RemoveSongFromPlaylist(int playlistId, int musicId)
        {
            var userId = GetCurrentUserId();
            // 1. Kiểm tra đăng nhập
            if (userId == 0)
            {
                return Json(new { success = false, error = "Chưa đăng nhập" });
            }

            // 2. (RẤT QUAN TRỌNG) Kiểm tra quyền sở hữu Playlist
            // Chúng ta dùng hàm GetPlaylistDetails vì nó đã có sẵn logic kiểm tra (UserID == userId)
            var playlist = _playlistService.GetPlaylistDetails(playlistId, userId);
            if (playlist == null)
            {
                return Json(new { success = false, error = "Bạn không có quyền xóa bài hát khỏi playlist này." });
            }

            // 3. Gọi Service để xóa
            bool success = _playlistService.RemoveSongFromPlaylist(playlistId, musicId);

            if (success)
            {
                return Json(new { success = true, message = "Đã xóa bài hát khỏi playlist." });
            }
            else
            {
                return Json(new { success = false, message = "Không tìm thấy bài hát trong playlist." });
            }
        }
        // === KẾT THÚC ACTION MỚI ===


        // (Hàm GetCurrentUserId() giữ nguyên)
        private int GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                string email = User.Identity.Name;
                var user = _userService.GetUserByEmail(email);
                return user?.UserID ?? 0;
            }
            return 0;
        }
    }
}