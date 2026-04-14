using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MUSICNOW.Core.ViewModels
{
    public class UploadMusicViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên bài hát")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên ca sĩ")]
        public string SingerName { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thể loại")]
        [Display(Name = "Thể loại")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn file nhạc")]
        [Display(Name = "File nhạc (.mp3)")]
        public HttpPostedFileBase MusicFile { get; set; }

        [Display(Name = "Ảnh bìa (JPG, PNG)")]
        public HttpPostedFileBase CoverArtFile { get; set; }
    }
}
