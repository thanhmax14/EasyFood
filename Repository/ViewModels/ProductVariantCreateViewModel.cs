using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ProductVariantCreateViewModel
    {
        public Guid ID { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public int Stock { get; set; }
        [DataType(DataType.Date)]
        public DateTime ManufactureDate { get; set; }
        public Guid ProductID { get; set; }
    }
}
