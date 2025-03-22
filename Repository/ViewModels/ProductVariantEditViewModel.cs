using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ProductVariantEditViewModel
    {
        public Guid ID { get; set; } // Chuyển sang kiểu GUID
        public Guid ProductID { get; set; } // Chuyển sang kiểu GUID
        public string Size { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public int Stock { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
