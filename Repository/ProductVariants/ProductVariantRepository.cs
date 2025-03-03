using Models.DBContext;
using Repository.BaseRepository;
using Repository.ProductVariants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ProductVariants
{
    public class ProductVariantRepository : BaseRepository<Models.ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(EasyFoodDbContext context) : base(context) { }
    }
}
