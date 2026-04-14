using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MUSICNOW.Core.Entities;


namespace MUSICNOW.Core.ViewModels
{
    // Lớp này dùng để truyền tất cả dữ liệu cần thiết cho trang chủ
    public class HomeViewModel
    {
        // 1. Dữ liệu cho khu vực "Bài hát thịnh hành"
        public List<TrackViewModel> TrendingTracks { get; set; }

        // 2. Dữ liệu cho khu vực "Bảng xếp hạng"
        public List<TrackViewModel> Top50VN { get; set; }
        public List<TrackViewModel> Top50Global { get; set; }

        // 3. Dữ liệu cho khu vực "Nghệ sĩ phổ biến" (Dữ liệu mẫu)
        public List<ProfileViewModel> PopularArtists { get; set; }
        public bool CanUpload { get; set; }
        public bool CanCreatePlaylist { get; set; }

        // Dùng để lưu ID của các bài hát user đã thích (để tô màu trái tim)
        public HashSet<int> LikedMusicIds { get; set; }

        // Dùng để hiển thị trong menu "Thêm vào playlist"
        public List<Playlist> UserPlaylists { get; set; }

        // Constructor khởi tạo các danh sách
        public HomeViewModel()
        {
            PopularArtists = new List<ProfileViewModel>();
            TrendingTracks = new List<TrackViewModel>();
            Top50VN = new List<TrackViewModel>();
            Top50Global = new List<TrackViewModel>();
        }


    }
}
