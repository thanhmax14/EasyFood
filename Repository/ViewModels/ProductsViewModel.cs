using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Repository.ViewModels
{
    public class ProductsViewModel
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
        public decimal Price { get; set; } = 0;

      
        public Guid CateID { get; set; }


        public List<string> Img { get; set; } = new List<string>();
        public Guid StoreId { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}
