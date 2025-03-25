using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class CategoryDetailsViewModel
    {
        public Guid ID { get; set; }
        public string Name { get; set; }

        public string? Img { get; set; }

        public List<ProductsViewModel> ProductViewModel { get; set; }

        public List<StoreDetailsViewModels> StoreDetailViewModel { get; set; }
        public int Number { get; set; } = 0;
        public float Commission { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
