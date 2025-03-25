using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class OrderActionViewModel
    {
        public Guid OrderId { get; set; }
        public string Action { get; set; } // "ACCEPT" hoặc "REJECT"
    }
}
