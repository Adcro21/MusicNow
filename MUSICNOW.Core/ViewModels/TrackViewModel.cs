using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUSICNOW.Core.ViewModels
{
    public class TrackViewModel
    {
        public int MusicID { get; set; }
        public string Title { get; set; }
        public string SingerName { get; set; }
        public string FilePath { get; set; }
        public string CoverArtUrl { get; set; }
        public int Views { get; set; }
    }

}
