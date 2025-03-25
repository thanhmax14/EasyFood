using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ProductIndexViewModel
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime ManufactureDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsOnSale { get; set; }
        public string CateName { get; set; }
        public string StoreName { get; set; }
        public string ImageUrl { get; set; } // Hình ảnh nào là IsMain
        public Guid StoreId { get; set; } // Thêm StoreId
        public bool StoreIsActive { get; set; } // Thêm trạng thái của Store
    }
}
