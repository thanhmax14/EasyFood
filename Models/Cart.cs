using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
   public class Cart
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        public int Quantity { get; set; }
        public DateTime? CreatedDate { get; set; }
        [ForeignKey("AppUser")]
        public string UserID { get; set; }
        public virtual AppUser AppUser { get; set; }
        [ForeignKey("ProductTypes")]
        public Guid ProductTypesID { get; set; }
        public virtual ProductTypes ProductTypes { get; set; }
    }
}
