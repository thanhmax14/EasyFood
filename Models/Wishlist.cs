using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Wishlist
    {
        [Key]
        public Guid ID { get; set; }= Guid.NewGuid();
        public DateTime? CreateDate { get; set; }
        [ForeignKey("AppUser")]
        public string UserID { get; set; }
        public virtual AppUser AppUser { get; set; }
        [ForeignKey("Product")]
        public Guid ProductID { get; set; }
        public virtual Product Product { get; set; }
    }
}
