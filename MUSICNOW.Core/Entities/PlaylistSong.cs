using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MUSICNOW.Core.Entities
{
    [Table("PlaylistSongs")]
    public class PlaylistSong
    {
        [Key]
        public int PlaylistSongID { get; set; }

        public int PlaylistID { get; set; }

        public int MusicID { get; set; }

        public DateTime AddedAt { get; set; }

        [ForeignKey("PlaylistID")]
        public virtual Playlist Playlist { get; set; }

        [ForeignKey("MusicID")]
        public virtual Music Music { get; set; }
    }
}
