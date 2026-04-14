using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MUSICNOW.Core.Entities;


namespace MUSICNOW.Core.ViewModels
{
    public class ForYouViewModel
    {
        // Cột trái
        public TrackViewModel FeaturedTrack { get; set; }

        // Cột phải
        public List<TrackViewModel> QueueTracks { get; set; }

        // Dữ liệu cho các nút điều khiển
        public HashSet<int> LikedMusicIds { get; set; }
        public List<Playlist> UserPlaylists { get; set; }

        public ForYouViewModel()
        {
            QueueTracks = new List<TrackViewModel>();
            LikedMusicIds = new HashSet<int>();
            UserPlaylists = new List<Playlist>();
        }
    }
}