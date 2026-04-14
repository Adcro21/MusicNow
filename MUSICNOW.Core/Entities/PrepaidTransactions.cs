using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MUSICNOW.Core.Entities
{
    [Table("PrepaidTransactions")]
    public class PrepaidTransactions
    {
        [Key]
        public int TransactionID { get; set; }
        public int? UserID { get; set; }
        public long Amount { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string PaymentMethod { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }
}
