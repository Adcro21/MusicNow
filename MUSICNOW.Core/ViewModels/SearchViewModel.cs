using System.Collections.Generic;
using System.Linq; // Cần cho .Any()

namespace MUSICNOW.Core.ViewModels
{
    public class SearchViewModel
    {
        // Từ khóa người dùng gõ
        public string SearchTerm { get; set; }

        // Danh sách các bài hát tìm thấy
        public List<TrackViewModel> Results { get; set; }

        // Hàm 'get' tiện ích để kiểm tra xem có kết quả không
        public bool HasResults => Results != null && Results.Any();

        // Constructor để luôn khởi tạo List, tránh lỗi Null
        public SearchViewModel()
        {
            Results = new List<TrackViewModel>();
        }
    }
}