using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ProductHideShowViewModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
