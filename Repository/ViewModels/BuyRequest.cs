using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class BuyRequest
    {
        public Dictionary<Guid, int> Products { get; set; } = new();
        public bool IsOnline { get; set; } = false;
        public string UserID { get; set; }
        public string SuccessUrl { get; set; }
        public string CalledUrl { get; set; }

    }
}
