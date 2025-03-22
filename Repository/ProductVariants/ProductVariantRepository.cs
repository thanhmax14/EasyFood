using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DBContext;
using Repository.BaseRepository;
using Repository.ProductVariants;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ProductVariants
{
    public class ProductVariantRepository : BaseRepository<Models.ProductVariant>, IProductVariantRepository
    {
        private readonly EasyFoodDbContext _context;
        public ProductVariantRepository(EasyFoodDbContext context) : base(context) {
            _context = context;
        }

        public async Task<List<ProductVariantViewModel>> GetVariantsByProductIdAsync(Guid productId)
        {
            var variants = await _context.ProductVariants
                .Where(v => v.ProductID == productId)
                .Select(v => new ProductVariantViewModel
                {
                    ID = v.ID,
                    Size = v.Size,
                    Price = v.Price,
                    OriginalPrice = v.OriginalPrice,
                    Stock = v.Stock,
                    ModifiedDate = v.ModifiedDate,
                    ManufactureDate = v.ManufactureDate,
                    ProductID = v.ProductID,
                    StoreID = v.Product.StoreID // Lấy StoreID từ Product
                })
                .ToListAsync();

            return variants;
        }

        public async Task CreateProductVariantAsync(ProductVariantCreateViewModel model)
        {
            var productVariant = new ProductVariant
            {
                ID = Guid.NewGuid(),
                Size = model.Size,
                Price = model.Price,
                OriginalPrice = model.OriginalPrice,
                Stock = model.Stock,
                ManufactureDate = model.ManufactureDate,
                ProductID = model.ProductID,
                ModifiedDate = DateTime.UtcNow
            };

            await _context.ProductVariants.AddAsync(productVariant);
            await _context.SaveChangesAsync();
        }
        public async Task<ProductVariantEditViewModel> GetProductVariantForEditAsync(Guid variantId)
        {
            return await _context.ProductVariants
                .Where(v => v.ID == variantId)
                .Select(v => new ProductVariantEditViewModel
                {
                    ID = v.ID,
                    ProductID = v.ProductID,
                    Size = v.Size,
                    Price = v.Price,
                    OriginalPrice = v.OriginalPrice,
                    Stock = v.Stock,
                    ManufactureDate = v.ManufactureDate,
                    ModifiedDate = v.ModifiedDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateProductVariantAsync(ProductVariantEditViewModel model)
        {
            var variant = await _context.ProductVariants.FindAsync(model.ID);
            if (variant == null) return false;

            variant.Size = model.Size;
            variant.Price = model.Price;
            variant.OriginalPrice = model.OriginalPrice;
            variant.Stock = model.Stock;
            variant.ManufactureDate = model.ManufactureDate;
            variant.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
