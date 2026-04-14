using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MUSICNOW.Core.Entities
{
    [Table("SupportTickets")]
    public class SupportTicket
    {
        [Key]
        public int TicketID { get; set; }
        public int? SubmittedBy { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? AssignedTo { get; set; }

        [ForeignKey("SubmittedBy")]
        public virtual User Submitter { get; set; }

        [ForeignKey("AssignedTo")]
        public virtual User Assignee { get; set; }
    }
}
