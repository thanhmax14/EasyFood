using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class GetSellerOrder
    {
        public int STT { get; set; }
        public Guid OrderID { get; set; }
        public string UserName { get; set; }
        public DateTime? OrderDate { get; set; }
        public int quantity { get; set; }
        public decimal total { get; set; }
        public string status { get; set; }
        public string shortID { get; set; }
    }
}
