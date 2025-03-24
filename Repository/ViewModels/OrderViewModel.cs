using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class OrderViewModel
    {
       

        public string Email { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
     //   public string Note { get; set; }
        public DateTime OrderDate { get; set; }
/*        public DateTime DeliveryDate { get; set; }*/
       
        public decimal Total { get; set; }

        public Guid OrderId { get; set; }

    }
}
