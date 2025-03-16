using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Voucher
    {
        [Key]
        public Guid ID { get; set; }
        public string Code { get; set; }
        public decimal DiscountAmount { get; set; }
        public string DiscountType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Scope { get; set; }
        public int MaxUsage { get; set; }
        public int CurrentUsage { get; set; }
        public decimal MinOrderValue { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public ICollection<Order> Orders { get; set; }

    }
}
