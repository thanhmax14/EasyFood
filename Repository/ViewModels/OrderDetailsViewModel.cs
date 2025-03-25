using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class OrderDetailsViewModel
    {
        public string productName { get; set; }
        public decimal ProductPrice { get; set; } = 0;
        public decimal TotalPrice { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public string? Status { get; set; }
        public Guid ProductId { get; set; }
    }
}
