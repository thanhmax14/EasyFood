using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ProductVariant
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Size { get; set; } // Kích thước (VD: "50g", "100g")
        public decimal Price { get; set; } = 0; // Giá bán
        public decimal? OriginalPrice { get; set; }
        public int Stock { get; set; } = 0;
        public DateTime? ModifiedDate { get; set; }
        public DateTime ManufactureDate { get; set; }// Ngày sản xuất
        public bool IsActive { get; set; } = true;
        [ForeignKey("Product")]
        public Guid ProductID { get; set; }
        public virtual Product Product { get; set; }
    }

}
