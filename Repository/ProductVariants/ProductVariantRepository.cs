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
    public class ProductVariantRepository : BaseRepository<Models.ProductTypes>, IProductVariantRepository
    {
        private readonly EasyFoodDbContext _context;
        public ProductVariantRepository(EasyFoodDbContext context) : base(context) {
            _context = context;
        }

        public async Task<List<ProductVariantViewModel>> GetVariantsByProductIdAsync(Guid productId)
        {
            var variants = await _context.ProductTypes
                .Where(v => v.ProductID == productId)
                .Select(v => new ProductVariantViewModel
                {
                    ID = v.ID,
                    Size = v.Name,
                    Price = v.SellPrice,
                    OriginalPrice = v.OriginalPrice,
                    Stock = v.Stock,
                    ModifiedDate = v.ModifiedDate,
                    ManufactureDate = v.ManufactureDate,
                    ProductID = v.ProductID,
                    IsActive = v.IsActive,
                    StoreID = v.Product.StoreID // Lấy StoreID từ Product
                })
                .ToListAsync();

            return variants;
        }

        public async Task CreateProductVariantAsync(ProductVariantCreateViewModel model)
        {
            var productVariant = new ProductTypes
            {
                ID = Guid.NewGuid(),
                Name = model.Size,
                SellPrice = model.Price,
                OriginalPrice = model.OriginalPrice,
                Stock = model.Stock,
                ManufactureDate = model.ManufactureDate,
                ProductID = model.ProductID,
                ModifiedDate = DateTime.UtcNow
            };

            await _context.ProductTypes.AddAsync(productVariant);
            await _context.SaveChangesAsync();
        }
        public async Task<ProductVariantEditViewModel> GetProductVariantForEditAsync(Guid variantId)
        {
            return await _context.ProductTypes
                .Where(v => v.ID == variantId)
                .Select(v => new ProductVariantEditViewModel
                {
                    ID = v.ID,
                    ProductID = v.ProductID,
                    Size = v.Name,
                    Price = v.SellPrice,
                    OriginalPrice = v.OriginalPrice,
                    Stock = v.Stock,
                    ManufactureDate = v.ManufactureDate,
                    ModifiedDate = v.ModifiedDate
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateProductVariantAsync(ProductVariantEditViewModel model)
        {
            var variant = await _context.ProductTypes.FindAsync(model.ID);
            if (variant == null) return false;

            variant.Name = model.Size;
            variant.SellPrice = model.Price;
            variant.OriginalPrice = model.OriginalPrice;
            variant.Stock = model.Stock;
            variant.ManufactureDate = model.ManufactureDate;
            variant.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public bool UpdateProductVariantStatus(Guid variantId, bool isActive)
        {
            var variant = _context.ProductTypes.FirstOrDefault(v => v.ID == variantId);
            if (variant == null)
                return false;

            variant.IsActive = isActive;
            _context.SaveChanges();
            return true;
        }
    }
}
