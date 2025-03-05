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
        public string Descriptions { get; set; }
        public string Status { get; set; }
        public string? Replay { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? DateReplay { get; set; }
        [ForeignKey("OrderDetail")]
        public Guid OrderDetailID { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
    }
}
