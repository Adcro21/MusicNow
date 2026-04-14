using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MUSICNOW.Core.Entities
{
    [Table("Logs")]
    public class Logs
    {
        [Key]
        public int LogID { get; set; }
        public int? UserID { get; set; }
        public string Action { get; set; }
        public DateTime? Timestamp { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }
}
