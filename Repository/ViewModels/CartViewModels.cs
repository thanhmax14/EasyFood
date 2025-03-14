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
        public float vote { get; set; }
        public float price { get; set; } = 0;
        public float Subtotal { get; set; }
    }
}
