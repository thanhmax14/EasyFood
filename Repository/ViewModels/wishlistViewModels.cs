using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class wishlistViewModels
    {
        public string? img { get; set; }
        public string? name { get; set; }
        public float vote { get; set; } = 100.0f;
        public decimal price { get; set; } = 0.0m;
        public Guid ProductID { get; set; }
        public Guid ID { get; set; }
    }
}
