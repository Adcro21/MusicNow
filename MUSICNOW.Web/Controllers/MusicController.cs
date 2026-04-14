using MUSICNOW.Core.Entities;
using MUSICNOW.Core.Interfaces;
using MUSICNOW.Core.ViewModels;
using MUSICNOW.Infrastructure.Data;
using MUSICNOW.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MUSICNOW.Web.Controllers
{
    [Authorize]
    public class MusicController : BaseController
    {
        public MusicController(
            IUserService userService,
            IMusicService musicService,
            IPlaylistService playlistService,
            ICategoryService categoryService,
            MusicNowDbContext context)
            : base(userService, musicService, playlistService, categoryService, context)
        {
        }
        // (Các Action Index, Upload, AJAX, LikedSongs, MyMusic - Giữ nguyên)
        #region Các Action cũ
        public ActionResult Index()
        {
            var trendingTracks = _musicService.GetTrendingMusic();
            var userId = GetCurrentUserId();

            var model = new HomeViewModel
            {
                TrendingTracks = trendingTracks,
                Top50Global = trendingTracks.Take(5).ToList(),
                Top50VN = trendingTracks.Take(5).ToList(),
                // Gọi hàm mới, không dùng dữ liệu giả
                PopularArtists = _userService.GetPopularArtists(5),
                CanUpload = (bool)ViewData["CanUpload"],
                CanCreatePlaylist = (bool)ViewData["CanCreatePlaylist"],
                LikedMusicIds = (userId > 0) ? _playlistService.GetLikedMusicIds(userId) : new HashSet<int>(),
                UserPlaylists = (ViewBag.UserPlaylists as List<Playlist>) ?? new List<Playlist>()
            };
            ViewBag.ActivePage = "NoiBat";
            return View(model);
        }
        [HttpGet]
        public ActionResult Upload()
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);
            if (user == null || !_musicService.CanUploadMusic(user.UserID))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "CategoryID", "CategoryName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(UploadMusicViewModel model)
        {
            var user = _userService.GetUserByEmail(User.Identity.Name);
            if (user == null || !_musicService.CanUploadMusic(user.UserID))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "CategoryID", "CategoryName");
            if (ModelState.IsValid)
            {
                if (model.MusicFile != null && model.MusicFile.ContentLength > 0)
                {
                    bool success = _musicService.UploadMusic(model, user.UserID);
                    if (success)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không thể lưu file nhạc.");
                    }
                }
                else
                {
                    ModelState.AddModelError("MusicFile", "Vui lòng chọn file nhạc.");
                }
            }
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult IncrementViewCount(int id)
        {
            _musicService.IncrementViewCount(id);
            return new EmptyResult();
        }
        [HttpPost]
        [Authorize]
        public JsonResult ToggleLike(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, error = "Chưa đăng nhập" });
            }
            bool isLiked = _playlistService.ToggleLike(userId, id);
            return Json(new { success = true, liked = isLiked });
        }
        [HttpPost]
        [Authorize]
        public JsonResult AddSongToPlaylist(int musicId, int playlistId)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Json(new { success = false, error = "Chưa đăng nhập" });
            }
            bool isAdded = _playlistService.AddSongToPlaylist(playlistId, musicId);
            if (isAdded)
            {
                return Json(new { success = true, message = "Đã thêm vào playlist." });
            }
            else
            {
                return Json(new { success = false, message = "Bài hát đã có trong playlist này." });
            }
        }
        [HttpGet]
        [Authorize]
        public ActionResult LikedSongs()
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return RedirectToAction("Login", "Account");
            }
            List<Music> likedSongsDb = _playlistService.GetLikedSongs(userId);
            var model = likedSongsDb.Select(music => new TrackViewModel
            {
                MusicID = music.MusicID,
                Title = music.Title,
                SingerName = music.SingerName,
                FilePath = music.FilePath,
                CoverArtUrl = music.CoverArtUrl,
                Views = music.Views // Cập nhật Views ở đây nếu cần
            }).ToList();
            ViewBag.ActivePage = "YeuThich";
            return View(model);
        }
        [HttpGet]
        [Authorize]
        public ActionResult MyMusic()
        {
            var userId = GetCurrentUserId();
            var user = _userService.GetUserByEmail(User.Identity.Name);
            var model = new List<TrackViewModel>();
            if (user != null && (user.Role == "MusicCreator" || user.Role == "Admin"))
            {
                model = _musicService.GetTrackViewModelsByUploader(userId);
            }
            ViewBag.ActivePage = "NhacCuaBan";
            return View(model);
        }
        #endregion

        // === ACTION EDIT (Giữ nguyên) ===
        #region Edit Music
        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            var userId = GetCurrentUserId();
            var user = _userService.GetUserByEmail(User.Identity.Name);
            var track = _musicService.GetTrackById(id);
            if (track == null)
            {
                return HttpNotFound();
            }
            var musicEntity = _context.Music.Find(id);
            if (musicEntity == null) return HttpNotFound();
            if (user.Role != "Admin" && musicEntity.UploadedBy != userId)
            {
                return RedirectToAction("MyMusic");
            }
            var model = new EditMusicViewModel
            {
                MusicID = track.MusicID,
                Title = track.Title,
                SingerName = track.SingerName,
                CategoryID = musicEntity.CategoryID.Value,
                ExistingCoverArtUrl = track.CoverArtUrl
            };
            ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "CategoryID", "CategoryName", model.CategoryID);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(EditMusicViewModel model)
        {
            var userId = GetCurrentUserId();
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "CategoryID", "CategoryName", model.CategoryID);
                return View(model);
            }
            bool success = _musicService.UpdateMusic(model, userId);
            if (success)
            {
                return RedirectToAction("MyMusic");
            }
            else
            {
                ModelState.AddModelError("", "Không thể cập nhật bài hát. Vui lòng thử lại.");
                ViewBag.Categories = new SelectList(_categoryService.GetAllCategories(), "CategoryID", "CategoryName", model.CategoryID);
                return View(model);
            }
        }
        #endregion

        // === THÊM 2 ACTION MỚI NÀY VÀO (Bước 3) ===
        #region Delete Music

        // GET: /Music/Delete/5
        [HttpGet]
        [Authorize]
        public ActionResult Delete(int id)
        {
            var userId = GetCurrentUserId();
            var user = _userService.GetUserByEmail(User.Identity.Name);
            var track = _musicService.GetTrackById(id); // Lấy ViewModel để hiển thị

            if (track == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra quyền sở hữu
            var musicEntity = _context.Music.Find(id);
            if (musicEntity == null) return HttpNotFound();

            if (user.Role != "Admin" && musicEntity.UploadedBy != userId)
            {
                return RedirectToAction("MyMusic"); // Không có quyền, đá về
            }

            return View(track); // Trả về View "Delete.cshtml" với model là TrackViewModel
        }

        // POST: /Music/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();

            // Gọi service Delete (đã bao gồm kiểm tra quyền)
            bool success = _musicService.DeleteMusic(id, userId);

            // Dù thành công hay thất bại, cũng quay về trang "Nhạc của bạn"
            return RedirectToAction("MyMusic");
        }

        #endregion

        // Hàm see all 
        [HttpGet]
        public ActionResult SeeAll(string listType)
        {
            if (string.IsNullOrEmpty(listType))
            {
                return RedirectToAction("Index");
            }

            ViewBag.Title = "Xem Tất Cả";
            object model = null;

            switch (listType.ToLower())
            {
                case "trending":
                    ViewBag.Title = "Bài Hát Thịnh Hành";
                    // Lấy nhiều hơn, ví dụ 50
                    model = _musicService.GetTrendingMusic();
                    break;
                case "top50vn":
                    ViewBag.Title = "Top 50 Việt Nam";
                    model = _musicService.GetTrendingMusic().Take(50).ToList();
                    break;
                case "artists":
                    ViewBag.Title = "Tất Cả Nghệ Sĩ";
                    // Lấy tất cả, ví dụ 100
                    model = _userService.GetPopularArtists(100);
                    break;
                default:
                    return RedirectToAction("Index");
            }

            // Chúng ta sẽ tạo 1 View "SeeAll.cshtml"
            // View này sẽ dùng dynamic model để tự kiểm tra kiểu dữ liệu
            return View(model);
        }

        // Kiểm tra trạng thái "like" của bài hát
        [HttpGet]
        [Authorize]
        public JsonResult CheckLikeStatus(int id) // id ở đây là musicId
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                // Chưa đăng nhập thì không thể "liked"
                return Json(new { isLiked = false }, JsonRequestBehavior.AllowGet);
            }

            // Gọi service chúng ta vừa tạo
            bool liked = _playlistService.IsSongLiked(userId, id);
            return Json(new { isLiked = liked }, JsonRequestBehavior.AllowGet);
        }


        // Action "Dành cho bạn"
        [HttpGet]
        public ActionResult ForYou()
        {
            var model = new ForYouViewModel();
            var userId = GetCurrentUserId();

            // 1. Lấy 20 bài hát ngẫu nhiên (đã có Views)
            var randomTracks = _musicService.GetRandomTracks(20);

            if (randomTracks != null && randomTracks.Any())
            {
                // 2. Chia ra: 1 bài nổi bật và 19 bài tiếp theo
                model.FeaturedTrack = randomTracks.FirstOrDefault();
                model.QueueTracks = randomTracks.Skip(1).ToList();
            }

            // 3. Lấy dữ liệu cho các nút (Like, Playlist)
            if (userId > 0)
            {
                model.LikedMusicIds = _playlistService.GetLikedMusicIds(userId);
                model.UserPlaylists = _playlistService.GetUserPlaylists(userId);
            }

            ViewBag.ActivePage = "DanhChoBan"; // Đặt active cho Sidebar
            return View(model);
        }

        // === XÓA BẢN SAO BỊ TRÙNG LẶP ===
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

        // (Bản sao bị trùng lặp ở đây đã được xóa)

    }
}