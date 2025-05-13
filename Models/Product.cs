using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Product
    {
        [Key]
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public DateTime ManufactureDate { get; set; }// Ngày sản xuất
        public bool IsActive { get; set; } = false;
        public bool IsOnSale { get; set; } // Có đang giảm giá?
        [ForeignKey("Categories")]
        public Guid CategoryID { get; set; }
        public virtual Categories Categories { get; set; }  
        [ForeignKey("StoreDetails")]
        public Guid StoreID { get; set; }
        public virtual StoreDetails StoreDetails { get; set; }
        public ICollection<ProductTypes> ProductTypes { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }

        public ICollection<Wishlist> Wishlists { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }


    }
}
