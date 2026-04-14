using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MUSICNOW.Core.Entities
{
    [Table("Music")]
    public class Music
    {
        [Key]
        public int MusicID { get; set; }

        public string Title { get; set; }
        public DateTime? UploadDate { get; set; }

        public int? UploadedBy { get; set; }
        public string SingerName { get; set; }
        public int? DurationInSeconds { get; set; }

        public int? CategoryID { get; set; }

        public string FilePath { get; set; } // Sửa thành string để lưu đường dẫn

        public int Views { get; set; }

        public string CoverArtUrl { get; set; }

        // Các khóa ngoại (Giúp EF liên kết dễ hơn)
        [ForeignKey("UploadedBy")]
        public virtual User Uploader { get; set; }

        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }
    }
}
