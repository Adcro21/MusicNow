using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MUSICNOW.Core.Entities
{
    [Table("Playlists")]
    public class Playlist
    {
        [Key]
        public int PlaylistID { get; set; }

        public int UserID { get; set; }

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        // Navigation property (Tùy chọn: Dùng để tham chiếu đến các bài hát trong playlist)
        public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; }

        //ảnh bìa 
        public string CoverArtUrl { get; set; }
    }
}
