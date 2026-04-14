using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MUSICNOW.Core.Entities
{
    [Table("UserLikes")]
    public class UserLike
    {
        [Key]
        public int LikeID { get; set; }

        public int UserID { get; set; }

        public int MusicID { get; set; }

        public DateTime LikedAt { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("MusicID")]
        public virtual Music Music { get; set; }
    }
}
