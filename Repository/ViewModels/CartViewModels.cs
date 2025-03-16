using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class CartViewModels
    {
        public Guid ProductID { get; set; }
        public int quantity { get; set; } = 0;
        public string? img { get; set; }
        public string? ProductName { get; set; }
        public float vote { get; set; }
        public decimal price { get; set; } = 0;
        public decimal Subtotal { get; set; }
        public int Stock { get; set; }
        public string UserID { get; set; }

    }
}
