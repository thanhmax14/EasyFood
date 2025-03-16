using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ProductListViewModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? LongDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime ManufactureDate { get; set; }// Ngày sản xuất
        public bool IsActive { get; set; } = false;
        public bool IsOnSale { get; set; } // Có đang giảm giá?
        public string? StoreName { get; set; }
        public string CategoryName { get; set; }
        public Guid CateID { get; set; }
        public List<ProductImageViewModel> Images { get; set; } // Danh sách hình ảnh (1 ảnh chính, 4 ảnh phụ)
        public ProductListViewModel()
        {
            Images = new List<ProductImageViewModel>();
        }
        public Guid StoreId { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}
