using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUSICNOW.Core.ViewModels
{
    public class ArtistDetailViewModel
    {
        public ProfileViewModel ArtistInfo { get; set; }
        public List<TrackViewModel> Tracks { get; set; }
    }
}
