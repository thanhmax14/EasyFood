using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ProductViewModel
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public DateTime ManufactureDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsOnSale { get; set; } = false;
        public Guid CateID { get; set; }
        public Guid StoreID { get; set; }
        public List<IFormFile> Images { get; set; } // Upload tối đa 5 hình
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
    }
}

