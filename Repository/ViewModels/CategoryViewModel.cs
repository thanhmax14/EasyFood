using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class CategoryViewModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        public string? Img { get; set; }

        public int Number { get; set; } = 0;
        public decimal Commission { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
