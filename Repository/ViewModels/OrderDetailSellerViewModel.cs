using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class OrderDetailSellerViewModel
    {
        public Guid OrderDetailID { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }

        public Guid ProductID { get; set; }
        public string ProductName { get; set; }

        public Guid OrderID { get; set; }

        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; } // Thêm số điện thoại
        public string AvatarUrl { get; set; } // Thêm hình đại diện

        public string ImageUrl { get; set; } // Thêm đường dẫn ảnh chính của sản phẩm
        public string Address { get; set; }
        public string Action { get; set; } // "ACCEPT" hoặc "REJECT"
        public string Status { get; set; }
    }
}
