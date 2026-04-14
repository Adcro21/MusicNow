using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MUSICNOW.Core.ViewModels
{
    public class EditPlaylistViewModel
    {
        [Required]
        public int PlaylistID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên playlist")]
        [Display(Name = "Tên Playlist")]
        public string Name { get; set; }

        // Dùng để hiển thị ảnh bìa hiện tại
        public string ExistingCoverArtUrl { get; set; }

        // Dùng để nhận file ảnh mới (không bắt buộc)
        [Display(Name = "Ảnh bìa mới (Nếu muốn thay)")]
        public HttpPostedFileBase CoverArtFile { get; set; }
    }
}