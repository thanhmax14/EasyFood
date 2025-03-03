using Models.DBContext;
using Repository.BaseRepository;
using Repository.Carts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ProductImage
{
    public class ProductImageRepository : BaseRepository<Models.ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(EasyFoodDbContext context) : base(context) { }
    }
}
