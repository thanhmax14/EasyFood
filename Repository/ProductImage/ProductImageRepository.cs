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
        public ProductImageRepository(EasyFoodDbContext context) : base(context) {
            _context = context;
        }
        private readonly EasyFoodDbContext _context;

        public async Task<bool> AddRangeAsync(IEnumerable<Models.ProductImage> productImages)
        {
            await _context.ProductImages.AddRangeAsync(productImages);
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
