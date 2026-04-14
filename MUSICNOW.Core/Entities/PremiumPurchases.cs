using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MUSICNOW.Core.Entities
{
    [Table("PremiumPurchases")]
    public class PremiumPurchases
    {
        [Key]
        public int PurchaseID { get; set; }
        public int? UserID { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public long Amount { get; set; }
        public DateTime ExpiryDate { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }
}
