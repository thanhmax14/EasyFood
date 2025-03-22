using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ProductUpdateViewModel
    {
        public Guid ProductID { get; set; } // ID sản phẩm (cần cho cập nhật)

        public string Name { get; set; }

        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }

        public DateTime ManufactureDate { get; set; }

        public DateTime? ModifiedDate { get; set; } // Ngày cập nhật sản phẩm

        public bool IsActive { get; set; } = true;
        public bool IsOnSale { get; set; } = false;

        [Required]
        public Guid CateID { get; set; }

        [Required]
        public Guid StoreID { get; set; }

        public List<IFormFile> NewImages { get; set; } = new(); // Ảnh mới khi cập nhật

        public List<string> ExistingImages { get; set; } = new(); // Ảnh đã lưu trước đó

        public List<string> RemoveImageUrls { get; set; } = new(); // Ảnh cần xóa khi cập nhật

        public List<SelectListItem> Categories { get; set; } = new();
        //public string ImageUrl { get; set; } // Thêm thuộc tính này
        public List<string> NewImageUrls { get; set; } = new();
    }
}
