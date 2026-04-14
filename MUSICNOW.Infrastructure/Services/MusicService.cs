using MUSICNOW.Core.Interfaces;
using MUSICNOW.Core.ViewModels;
using MUSICNOW.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace MUSICNOW.Infrastructure.Services
{
    public class MusicService : IMusicService
    {
        // 1. KHAI BÁO BIẾN (Không dùng từ khóa 'new' ở đây nữa)
        private readonly MusicNowDbContext _context;

        // 2. CONSTRUCTOR INJECTION: Unity DI Container sẽ tự động "bơm" DbContext vào đây
        public MusicService(MusicNowDbContext context)
        {
            _context = context;
        }

        #region Các hàm Map và Get cơ bản

        private TrackViewModel MapMusicToTrackViewModel(Core.Entities.Music music)
        {
            if (music == null) return null;
            return new TrackViewModel
            {
                MusicID = music.MusicID,
                Title = music.Title,
                SingerName = music.SingerName,
                FilePath = music.FilePath,
                CoverArtUrl = music.CoverArtUrl,
                Views = music.Views
            };
        }

        public List<TrackViewModel> GetMusicByCategory(int categoryId)
        {
            var musicInCategoryData = _context.Music
                .Where(m => m.CategoryID == categoryId)
                .Select(m => new { m.MusicID, m.Title, m.SingerName, m.FilePath, m.CoverArtUrl })
                .ToList();
            var musicTracks = musicInCategoryData.Select(m => new TrackViewModel
            {
                MusicID = m.MusicID,
                Title = m.Title,
                SingerName = m.SingerName,
                FilePath = m.FilePath,
                CoverArtUrl = m.CoverArtUrl
            }).ToList();
            return musicTracks;
        }

        public TrackViewModel GetTrackById(int musicId)
        {
            var music = _context.Music.Find(musicId);
            return MapMusicToTrackViewModel(music);
        }

        public List<TrackViewModel> GetTrendingMusic()
        {
            var trendingMusicData = _context.Music
                .OrderByDescending(m => m.Views)
                .Take(10)
                .Select(m => new { m.MusicID, m.Title, m.SingerName, m.FilePath, m.CoverArtUrl })
                .ToList();
            var trendingTracks = trendingMusicData.Select(m => new TrackViewModel
            {
                MusicID = m.MusicID,
                Title = m.Title,
                SingerName = m.SingerName,
                FilePath = m.FilePath,
                CoverArtUrl = m.CoverArtUrl
            }).ToList();
            return trendingTracks;
        }

        public List<TrackViewModel> GetTrackViewModelsByUploader(int userId)
        {
            var musicEntities = _context.Music
                .Where(m => m.UploadedBy == userId)
                .OrderByDescending(m => m.UploadDate)
                .ToList();
            var viewModels = musicEntities
                .Select(music => MapMusicToTrackViewModel(music))
                .ToList();
            return viewModels;
        }

        #endregion

        #region Các nghiệp vụ Upload, Update, Delete

        public bool CanUploadMusic(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return false;
            return user.Role == "MusicCreator" || user.Role == "Admin";
        }

        public bool UploadMusic(UploadMusicViewModel model, int uploaderId)
        {
            if (model.MusicFile == null || model.MusicFile.ContentLength <= 0)
            {
                return false;
            }
            string musicRelativePath;
            try
            {
                string fileExtension = Path.GetExtension(model.MusicFile.FileName).ToLower();
                string fileName = $"track_{uploaderId}_{DateTime.Now.Ticks}{fileExtension}";
                string serverFolderPath = HttpContext.Current.Server.MapPath("~/Uploads/Music/");
                if (!Directory.Exists(serverFolderPath))
                {
                    Directory.CreateDirectory(serverFolderPath);
                }
                string fullPath = Path.Combine(serverFolderPath, fileName);
                model.MusicFile.SaveAs(fullPath);
                musicRelativePath = $"/Uploads/Music/{fileName}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi lưu file nhạc: {ex.Message}");
                return false;
            }

            string coverArtRelativePath = null;
            if (model.CoverArtFile != null && model.CoverArtFile.ContentLength > 0)
            {
                try
                {
                    string imgExtension = Path.GetExtension(model.CoverArtFile.FileName).ToLower();
                    string imgFileName = $"cover_{uploaderId}_{DateTime.Now.Ticks}{imgExtension}";
                    string imgServerFolderPath = HttpContext.Current.Server.MapPath("~/Uploads/CoverArt/");
                    if (!Directory.Exists(imgServerFolderPath))
                    {
                        Directory.CreateDirectory(imgServerFolderPath);
                    }
                    string imgFullPath = Path.Combine(imgServerFolderPath, imgFileName);
                    model.CoverArtFile.SaveAs(imgFullPath);
                    coverArtRelativePath = $"/Uploads/CoverArt/{imgFileName}";
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi lưu ảnh bìa: {ex.Message}");
                }
            }

            var newTrack = new MUSICNOW.Core.Entities.Music
            {
                Title = model.Title,
                UploadedBy = uploaderId,
                SingerName = model.SingerName,
                DurationInSeconds = 0,
                CategoryID = model.CategoryID,
                FilePath = musicRelativePath,
                CoverArtUrl = coverArtRelativePath,
                UploadDate = DateTime.Now,
                Views = 0
            };

            try
            {
                _context.Music.Add(newTrack);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi lưu nhạc vào DB: {ex.Message}");
                return false;
            }
        }

        public bool UpdateMusic(EditMusicViewModel model, int userId)
        {
            var music = _context.Music.Find(model.MusicID);
            if (music == null) return false;
            var user = _context.Users.Find(userId);
            if (user.Role != "Admin" && music.UploadedBy != userId)
            {
                return false;
            }

            music.Title = model.Title;
            music.SingerName = model.SingerName;
            music.CategoryID = model.CategoryID;

            if (model.CoverArtFile != null && model.CoverArtFile.ContentLength > 0)
            {
                if (!string.IsNullOrEmpty(music.CoverArtUrl))
                {
                    try
                    {
                        var oldFilePath = HttpContext.Current.Server.MapPath(music.CoverArtUrl);
                        if (File.Exists(oldFilePath))
                        {
                            File.Delete(oldFilePath);
                        }
                    }
                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Lỗi xóa ảnh cũ: {ex.Message}"); }
                }
                try
                {
                    string imgExtension = Path.GetExtension(model.CoverArtFile.FileName).ToLower();
                    string imgFileName = $"cover_{userId}_{DateTime.Now.Ticks}{imgExtension}";
                    string imgServerFolderPath = HttpContext.Current.Server.MapPath("~/Uploads/CoverArt/");
                    if (!Directory.Exists(imgServerFolderPath))
                    {
                        Directory.CreateDirectory(imgServerFolderPath);
                    }
                    string imgFullPath = Path.Combine(imgServerFolderPath, imgFileName);
                    model.CoverArtFile.SaveAs(imgFullPath);
                    music.CoverArtUrl = $"/Uploads/CoverArt/{imgFileName}";
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Lỗi lưu ảnh mới: {ex.Message}"); }
            }

            try
            {
                _context.Entry(music).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi cập nhật DB: {ex.Message}");
                return false;
            }
        }

        public bool DeleteMusic(int musicId, int userId)
        {
            var music = _context.Music.Find(musicId);
            if (music == null) return false;
            var user = _context.Users.Find(userId);
            if (user.Role != "Admin" && music.UploadedBy != userId)
            {
                return false;
            }

            try
            {
                var inPlaylists = _context.PlaylistSongs.Where(ps => ps.MusicID == musicId);
                _context.PlaylistSongs.RemoveRange(inPlaylists);

                var inLikes = _context.UserLikes.Where(l => l.MusicID == musicId);
                _context.UserLikes.RemoveRange(inLikes);

                if (!string.IsNullOrEmpty(music.CoverArtUrl))
                {
                    var artPath = HttpContext.Current.Server.MapPath(music.CoverArtUrl);
                    if (File.Exists(artPath)) File.Delete(artPath);
                }
                if (!string.IsNullOrEmpty(music.FilePath))
                {
                    var musicPath = HttpContext.Current.Server.MapPath(music.FilePath);
                    if (File.Exists(musicPath)) File.Delete(musicPath);
                }

                _context.Music.Remove(music);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi khi xóa bài hát: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Các tính năng bổ sung (View, Search, Random)

        public void IncrementViewCount(int musicId)
        {
            var music = _context.Music.Find(musicId);
            if (music != null)
            {
                music.Views += 1;
                _context.SaveChanges();
            }
        }

        public List<TrackViewModel> SearchTracks(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return new List<TrackViewModel>();
            }

            var term = searchTerm.ToLower();

            var musicEntities = _context.Music
                .Where(m =>
                    m.Title.ToLower().Contains(term) ||
                    m.SingerName.ToLower().Contains(term)
                )
                .OrderByDescending(m => m.Views)
                .ToList();

            var viewModels = musicEntities
                .Select(music => MapMusicToTrackViewModel(music))
                .ToList();

            return viewModels;
        }

        public List<TrackViewModel> GetRandomTracks(int count)
        {
            var musicEntities = _context.Music
                .OrderBy(m => Guid.NewGuid())
                .Take(count)
                .ToList();

            var viewModels = musicEntities
                .Select(music => MapMusicToTrackViewModel(music))
                .ToList();

            return viewModels;
        }

        #endregion
    }
}