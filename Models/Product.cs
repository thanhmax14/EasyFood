using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Product
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public string? Tags { get; set; } // Tag (VD: "Organic, Brown")
        public string? Type { get; set; } // Loại sản phẩm (VD: "Organic")
        public DateTime ManufactureDate { get; set; }// Ngày sản xuất
        public bool IsAvailable { get; set; } = true;
        public bool IsOnSale { get; set; } // Có đang giảm giá?
        [ForeignKey("Categories")]
        public Guid CateID { get; set; }
        public virtual Categories Categories { get; set; }  
        [ForeignKey("StoreDetails")]
        public Guid StoreID { get; set; }
        public virtual StoreDetails StoreDetails { get; set; }
        public ICollection<ProductVariant> ProductVariants { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
    
        
        

    }
}
