using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BalanceChange
    {
        [Key]
        public Guid ID { get; set; }=Guid.NewGuid();
        public decimal MoneyBeforeChange { get; set; } = 0;
        public decimal MoneyChange { get; set; } = 0;
        public decimal MoneyAfterChange { get; set; } = 0;
        public DateTime? StartTime { get; set; }
        public DateTime? DueTime { get; set; }
        public string? Description { get; set; }
        [ForeignKey("AppUser")]
        public string UserID { get; set; }
        public string Status { get; set; }
        public string Method { get; set; }
        public bool Display { get; set; } = true;
        public int? OrderCode { get; set; }
        public bool IsComplete { get; set; } = false;
        public bool CheckDone { get; set; } = false;
        public virtual AppUser AppUser { get; set; }
    }
}
