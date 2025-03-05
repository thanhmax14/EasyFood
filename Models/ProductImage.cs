using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ProductImage
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; } = false;
        [ForeignKey("Product")]
        public Guid ProductID { get; set; }
        public virtual Product Product { get; set; }
    }
}
