using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MUSICNOW.Core.ViewModels
{
    public class EditMusicViewModel
    {
        [Required]
        public int MusicID { get; set; } // << Quan trọng

        [Required(ErrorMessage = "Vui lòng nhập tên bài hát")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên ca sĩ")]
        public string SingerName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thể loại")]
        [Display(Name = "Thể loại")]
        public int CategoryID { get; set; }

        // Dùng để hiển thị ảnh bìa hiện tại
        public string ExistingCoverArtUrl { get; set; }

        // === CÁC FILE KHÔNG BẮT BUỘC ===

        [Display(Name = "Tải lên ảnh bìa mới (Nếu muốn thay)")]
        public HttpPostedFileBase CoverArtFile { get; set; } // Không 'Required'

        // Chúng ta sẽ không cho phép thay đổi file nhạc (FilePath)
        // vì nó quá phức tạp (cần xóa file cũ, v.v.).
        // Chức năng "Chỉnh sửa" thường chỉ là metadata (tên, ảnh).
    }
}