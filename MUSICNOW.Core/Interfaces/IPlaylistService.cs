using MUSICNOW.Core.Entities;
using MUSICNOW.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUSICNOW.Core.Interfaces
{
    public interface IPlaylistService
    {
        // 1. Logic kiểm tra quyền hạn (Dùng để ẩn/hiện nút Tạo Playlist)
        bool CanCreateNewPlaylist(int userId);

        // 2. Hành động tạo Playlist
        int CreatePlaylist(int userId, string name);

        // 3. Hành động quản lý Playlist
        bool AddSongToPlaylist(int playlistId, int musicId);

        // 4. Hành động quản lý Yêu thích
        bool ToggleLike(int userId, int musicId);

        // *Tùy chọn: Lấy danh sách Playlist của User*
        List<Playlist> GetUserPlaylists(int userId);
        //  Lấy danh sách ID bài hát yêu thích của User
        HashSet<int> GetLikedMusicIds(int userId);
        // Lấy danh sách bài hát yêu thích của User
        List<Music> GetLikedSongs(int userId); // Trả về List<Music>
        // Lấy chi tiết Playlist
        Playlist GetPlaylistDetails(int playlistId, int userId);

        // Lấy danh sách bài hát trong Playlist
        List<TrackViewModel> GetSongsInPlaylist(int playlistId);
        // Hành động chỉnh sửa Playlist

        bool UpdatePlaylist(EditPlaylistViewModel model, int userId);
        // Hành động xóa Playlist
        bool DeletePlaylist(int playlistId, int userId);
        // Hành động xóa bài hát khỏi Playlist
        bool RemoveSongFromPlaylist(int playlistId, int musicId);
        // Kiểm tra bài hát đã được yêu thích hay chưa
        bool IsSongLiked(int userId, int musicId);
    }
}