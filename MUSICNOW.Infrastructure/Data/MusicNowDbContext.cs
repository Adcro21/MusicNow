using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MUSICNOW.Core.Entities; 
using System.Data.Entity;

namespace MUSICNOW.Infrastructure.Data
{
    public class MusicNowDbContext : DbContext
    {
        // Tên "MusicNowDBEntities" phải khớp với Web.config
        public MusicNowDbContext() : base("name=MusicNowDBEntities")
        {
            Database.SetInitializer<MusicNowDbContext>(null);
        }

        public DbSet<User> Users { get; set; } // Phải có dòng này
        public DbSet<Category> Categories { get; set; }
        public DbSet<Music> Music { get; set; }
        public DbSet<PremiumPurchases> PremiumPurchases { get; set; }
        public DbSet<PrepaidTransactions> PrepaidTransactions { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<Logs> Logs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
        public DbSet<UserLike> UserLikes { get; set; }
    }
}
