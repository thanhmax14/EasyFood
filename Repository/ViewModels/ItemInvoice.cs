using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ItemInvoice
    {
        public string nameItem { get; set; }
        public decimal unitPrice { get; set; } = 0;
        public int quantity { get; set; } = 0;
        public decimal amount { get; set; } = 0;

    }
}
