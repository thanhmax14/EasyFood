using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ProductVariantViewModel
    {
        public Guid ID { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int Stock { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime ManufactureDate { get; set; }
        public Guid ProductID { get; set; }
        public Guid StoreID { get; set; } // 🆕 Thêm StoreID
    }
}
