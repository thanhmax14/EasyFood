using Repository.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ProductVariants
{
    public interface IProductVariantRepository : IBaseRepository<Models.ProductTypes>
    {
    }
}
