using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MUSICNOW.Core.Interfaces;
using MUSICNOW.Infrastructure.Data;
using MUSICNOW.Core.Entities;
using System.Data.Entity;
using MUSICNOW.Core.ViewModels;
using System.IO;
using System.Web;

namespace MUSICNOW.Infrastructure.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly MusicNowDbContext _context;

        // Sử dụng Constructor Injection: Nhận context từ bên ngoài
        public PlaylistService(MusicNowDbContext context)
        {
            _context = context;
        }

        // (Các hàm 1-11 cũ giữ nguyên)
        #region Các hàm cũ (Create, Add, Like, Get, Update, Delete...)
        public bool CanCreateNewPlaylist(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return false;
            if (user.IsPremium || user.Role == "Admin" || user.Role == "MusicCreator")
            {
                return true;
            }
            return false;
        }

        public int CreatePlaylist(int userId, string name)
        {
            if (!CanCreateNewPlaylist(userId))
            {
                return 0;
            }
            var newPlaylist = new Playlist
            {
                UserID = userId,
                Name = name,
                CreatedAt = DateTime.Now
            };
            _context.Playlists.Add(newPlaylist);
            _context.SaveChanges();
            return newPlaylist.PlaylistID;
        }

        public bool AddSongToPlaylist(int playlistId, int musicId)
        {
            if (_context.PlaylistSongs.Any(ps => ps.PlaylistID == playlistId && ps.MusicID == musicId))
            {
                return false;
            }
            var newSong = new PlaylistSong
            {
                PlaylistID = playlistId,
                MusicID = musicId,
                AddedAt = DateTime.Now
            };
            _context.PlaylistSongs.Add(newSong);
            var music = _context.Music.Find(musicId);
            if (music != null)
            {
                music.Views += 1;
            }
            _context.SaveChanges();
            return true;
        }

        public bool ToggleLike(int userId, int musicId)
        {
            var existingLike = _context.UserLikes
                .FirstOrDefault(l => l.UserID == userId && l.MusicID == musicId);

            if (existingLike != null)
            {
                _context.UserLikes.Remove(existingLike);
                _context.SaveChanges();
                return false;
            }
            else
            {
                var newLike = new UserLike
                {
                    UserID = userId,
                    MusicID = musicId,
                    LikedAt = DateTime.Now
                };
                _context.UserLikes.Add(newLike);
                _context.SaveChanges();
                return true;
            }
        }

        public List<Playlist> GetUserPlaylists(int userId)
        {
            return _context.Playlists.Where(p => p.UserID == userId).ToList();
        }

        public HashSet<int> GetLikedMusicIds(int userId)
        {
            return _context.UserLikes
                .Where(l => l.UserID == userId)
                .Select(l => l.MusicID)
                .ToHashSet();
        }

        public List<Music> GetLikedSongs(int userId)
        {
            var likedMusicIds = _context.UserLikes
                .Where(l => l.UserID == userId)
                .Select(l => l.MusicID)
                .ToList();
            return _context.Music
                .Where(m => likedMusicIds.Contains(m.MusicID))
                .ToList();
        }

        public Playlist GetPlaylistDetails(int playlistId, int userId)
        {
            return _context.Playlists.FirstOrDefault(p => p.PlaylistID == playlistId && p.UserID == userId);
        }

        public List<TrackViewModel> GetSongsInPlaylist(int playlistId)
        {
            var musicEntities = _context.PlaylistSongs
                .Where(ps => ps.PlaylistID == playlistId)
                .Include(ps => ps.Music)
                .Select(ps => ps.Music)
                .ToList();

            var viewModels = musicEntities.Select(music => new TrackViewModel
            {
                MusicID = music.MusicID,
                Title = music.Title,
                SingerName = music.SingerName,
                FilePath = music.FilePath,
                CoverArtUrl = music.CoverArtUrl
            }).ToList();

            return viewModels;
        }

        public bool UpdatePlaylist(EditPlaylistViewModel model, int userId)
        {
            var playlist = _context.Playlists.Find(model.PlaylistID);
            if (playlist == null) return false;
            if (playlist.UserID != userId)
            {
                return false;
            }
            playlist.Name = model.Name;
            if (model.CoverArtFile != null && model.CoverArtFile.ContentLength > 0)
            {
                if (!string.IsNullOrEmpty(playlist.CoverArtUrl))
                {
                    try
                    {
                        var oldFilePath = HttpContext.Current.Server.MapPath(playlist.CoverArtUrl);
                        if (File.Exists(oldFilePath))
                        {
                            File.Delete(oldFilePath);
                        }
                    }
                    catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Lỗi xóa ảnh cũ playlist: {ex.Message}"); }
                }
                try
                {
                    string imgExtension = Path.GetExtension(model.CoverArtFile.FileName).ToLower();
                    string imgFileName = $"playlist_{model.PlaylistID}_{DateTime.Now.Ticks}{imgExtension}";
                    string imgServerFolderPath = HttpContext.Current.Server.MapPath("~/Uploads/CoverArt/");

                    if (!Directory.Exists(imgServerFolderPath))
                    {
                        Directory.CreateDirectory(imgServerFolderPath);
                    }

                    string imgFullPath = Path.Combine(imgServerFolderPath, imgFileName);
                    model.CoverArtFile.SaveAs(imgFullPath);
                    playlist.CoverArtUrl = $"/Uploads/CoverArt/{imgFileName}";
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Lỗi lưu ảnh mới playlist: {ex.Message}"); }
            }
            try
            {
                _context.Entry(playlist).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi cập nhật playlist DB: {ex.Message}");
                return false;
            }
        }

        public bool DeletePlaylist(int playlistId, int userId)
        {
            var playlist = _context.Playlists.Find(playlistId);
            if (playlist == null) return false;
            if (playlist.UserID != userId)
            {
                return false;
            }
            var songsInPlaylist = _context.PlaylistSongs.Where(ps => ps.PlaylistID == playlistId);
            _context.PlaylistSongs.RemoveRange(songsInPlaylist);
            if (!string.IsNullOrEmpty(playlist.CoverArtUrl))
            {
                try
                {
                    var oldFilePath = HttpContext.Current.Server.MapPath(playlist.CoverArtUrl);
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Lỗi xóa ảnh playlist: {ex.Message}"); }
            }
            _context.Playlists.Remove(playlist);
            _context.SaveChanges();
            return true;
        }
        #endregion

        // === THÊM HÀM MỚI NÀY VÀO (Bước 2) ===
        public bool RemoveSongFromPlaylist(int playlistId, int musicId)
        {
            // 1. Tìm bản ghi liên kết
            var songLink = _context.PlaylistSongs
                .FirstOrDefault(ps => ps.PlaylistID == playlistId && ps.MusicID == musicId);

            if (songLink == null)
            {
                return false; // Bài hát không có trong playlist
            }

            // 2. Xóa bản ghi liên kết đó
            _context.PlaylistSongs.Remove(songLink);
            _context.SaveChanges();

            return true;
        }

        // Kiểm tra bài hát đã được yêu thích hay chưa
        public bool IsSongLiked(int userId, int musicId)
        {
            return _context.UserLikes.Any(l => l.UserID == userId && l.MusicID == musicId);
        }


    } // Đóng class PlaylistService
} // Đóng namespace