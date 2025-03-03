using Models.DBContext;
using Repository.BaseRepository;
using Repository.StoreDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Products
{
    public class ProductsRepository : BaseRepository<Models.Product>, IProductsRepository
    {
        public ProductsRepository(EasyFoodDbContext context) : base(context) { }
    }
}
