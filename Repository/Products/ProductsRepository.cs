using Microsoft.EntityFrameworkCore;
using Models;
using Models.DBContext;
using Repository.BaseRepository;
using Repository.StoreDetails;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Repository.Products
{
    public class ProductsRepository : BaseRepository<Models.Product>, IProductsRepository
    {
        private readonly EasyFoodDbContext _context;
        public ProductsRepository(EasyFoodDbContext context) : base(context) {
            _context = context;
        }
        
        public async Task<bool> AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Guid> GetCurrentStoreIDAsync(string userId)
        {
            var store = await _context.StoreDetails.FirstOrDefaultAsync(s => s.UserID == userId);
            return store.ID;
        }

        public async Task<List<ProductListViewModel>> GetProductsWithDetailsByStoreIdAsync(Guid storeId)
        {
            // Bước 1: Lấy danh sách sản phẩm (không có ảnh)
            var productEntities = await (from p in _context.Products
                                         join c in _context.Categories on p.CateID equals c.ID
                                         join s in _context.StoreDetails on p.StoreID equals s.ID
                                         where p.StoreID == storeId
                                         select new ProductListViewModel
                                         {
                                             ID = p.ID,
                                             Name = p.Name,
                                             ShortDescription = p.ShortDescription,
                                             LongDescription = p.LongDescription,
                                             CreatedDate = p.CreatedDate,
                                             ModifiedDate = p.ModifiedDate,
                                             ManufactureDate = p.ManufactureDate,
                                             IsActive = p.IsActive,
                                             IsOnSale = p.IsOnSale,
                                             CategoryName = c.Name,
                                             StoreName = s.Name,
                                             StoreId = p.StoreID,
                                             CateID = p.CateID,
                                             Images = new List<ProductImageViewModel>() // Tạo danh sách rỗng ban đầu
                                         }).ToListAsync();

            // Bước 2: Lấy danh sách ảnh của tất cả sản phẩm
            var productIds = productEntities.Select(p => p.ID).ToList();
            var images = await _context.ProductImages
                .Where(img => productIds.Contains(img.ProductID))
                .OrderByDescending(img => img.IsMain)
                .ThenByDescending(img => img.ID)
                .ToListAsync();

            // Bước 3: Nhóm ảnh theo từng sản phẩm
            var imageDict = images.GroupBy(img => img.ProductID)
                                  .ToDictionary(g => g.Key, g => g
                                      .Take(5)
                                      .Select(img => new ProductImageViewModel
                                      {
                                          ImageUrl = img.ImageUrl,
                                          IsMain = img.IsMain
                                      })
                                      .ToList());

            // Bước 4: Gán danh sách ảnh vào sản phẩm tương ứng
            foreach (var product in productEntities)
            {
                if (imageDict.TryGetValue(product.ID, out var imgList))
                {
                    product.Images = imgList;
                }
            }

            return productEntities;
        }
    }
}
