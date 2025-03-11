using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class StoreViewModel
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? Call { get; set; }
    }
}
