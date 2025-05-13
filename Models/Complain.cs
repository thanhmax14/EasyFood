using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Complain
    {
        [Key]
        public Guid ID { get; set; }=Guid.NewGuid();
        public string Description { get; set; }
        public string Status { get; set; }
        public string? Reply { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ReplyDate { get; set; }
        [ForeignKey("OrderDetail")]
        public Guid OrderDetailID { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
    }
}
