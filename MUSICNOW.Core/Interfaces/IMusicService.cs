using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MUSICNOW.Core.Entities;
using MUSICNOW.Core.ViewModels;


namespace MUSICNOW.Core.Interfaces
{
    public interface IMusicService
    {
        // Lấy danh sách nhạc 
        TrackViewModel GetTrackById(int musicId);
        // Lấy danh sách nhạc thịnh hành
        List<TrackViewModel> GetTrendingMusic();
        // Lấy danh sách nhạc theo thể loại
        List<TrackViewModel> GetMusicByCategory(int categoryId);
        // Kiểm tra xem người dùng có thể tải lên nhạc không
        bool CanUploadMusic(int userId);
        // Tải lên nhạc mới
        bool UploadMusic(UploadMusicViewModel model, int uploaderId);
        //Tính Views 
        void IncrementViewCount(int musicId);
        // Lấy danh sách nhạc của người dùng tải lên
        List<TrackViewModel> GetTrackViewModelsByUploader(int userId);
        // Chỉnh sửa thông tin nhạc
        bool UpdateMusic(EditMusicViewModel model, int userId);
        // Xóa nhạc
        bool DeleteMusic(int musicId, int userId);
        // Tìm kiếm nhạc theo từ khóa
        List<TrackViewModel> SearchTracks(string searchTerm);
        // Lấy danh sách nhạc ngẫu nhiên
        List<TrackViewModel> GetRandomTracks(int count);
    }
}
