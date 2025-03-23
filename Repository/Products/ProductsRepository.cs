using Microsoft.AspNetCore.Http;
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

        public List<ProductIndexViewModel> GetProductsByStoreId(Guid storeId)
        {
            var products = (from p in _context.Products
                            join c in _context.Categories on p.CateID equals c.ID
                            join s in _context.StoreDetails on p.StoreID equals s.ID
                            join i in _context.ProductImages.Where(img => img.IsMain)
                                on p.ID equals i.ProductID into imgGroup
                            from img in imgGroup.DefaultIfEmpty() // Lấy ảnh IsMain nếu có
                            where p.StoreID == storeId
                            select new ProductIndexViewModel
                            {
                                ProductId = p.ID,
                                Name = p.Name,
                                ShortDescription = p.ShortDescription,
                                LongDescription = p.LongDescription,
                                CreatedDate = p.CreatedDate,
                                ModifiedDate = p.ModifiedDate,
                                ManufactureDate = p.ManufactureDate,
                                IsActive = p.IsActive,
                                IsOnSale = p.IsOnSale,
                                CateName = c.Name,
                                StoreName = s.Name,
                                ImageUrl = img != null ? img.ImageUrl : "/images/default.png"
                            }).ToList();

            return products;
        }
        public async Task<Guid> CreateProductAsync(Product product, List<Models.ProductImage> images)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            if (images.Any())
            {
                foreach (var image in images)
                {
                    image.ProductID = product.ID; // Gán ProductID
                    await _context.ProductImages.AddAsync(image);
                }
                await _context.SaveChangesAsync();
            }

            return product.ID;
        }

        public async Task<List<Categories>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<ProductUpdateViewModel> GetProductByIdAsync(Guid productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ID == productId);

            if (product == null) return null;

            return new ProductUpdateViewModel
            {
                ProductID = product.ID,
                Name = product.Name,
                ShortDescription = product.ShortDescription,
                LongDescription = product.LongDescription,
                ManufactureDate = product.ManufactureDate,
                ModifiedDate = product.ModifiedDate,
                IsActive = product.IsActive,
                IsOnSale = product.IsOnSale,
                CateID = product.CateID,
                StoreID = product.StoreID,
                ExistingImages = product.ProductImages.Select(i => i.ImageUrl).ToList()
            };
        }

        public async Task UpdateProductAsync(ProductUpdateViewModel model, List<IFormFile> newImages, string webRootPath)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ID == model.ProductID);

            if (product == null) return;

            product.Name = model.Name;
            product.ShortDescription = model.ShortDescription;
            product.LongDescription = model.LongDescription;
            product.ManufactureDate = model.ManufactureDate;
            product.ModifiedDate = DateTime.UtcNow;
            //product.IsActive = model.IsActive;
            product.IsOnSale = model.IsOnSale;
            product.CateID = model.CateID;

            // Xóa ảnh cũ nếu có trong danh sách RemoveImageUrls
            if (model.RemoveImageUrls?.Any() == true)
            {
                var imagesToRemove = product.ProductImages
                    .Where(i => model.RemoveImageUrls.Contains(i.ImageUrl))
                    .ToList();

                foreach (var img in imagesToRemove)
                {
                    string imgPath = Path.Combine(webRootPath, "uploads", img.ImageUrl);
                    if (File.Exists(imgPath))
                    {
                        File.Delete(imgPath);
                    }
                }

                _context.ProductImages.RemoveRange(imagesToRemove);
                await _context.SaveChangesAsync();
            }

            // Kiểm tra lại số lượng ảnh sau khi xóa
            int currentImageCount = product.ProductImages.Count;
            bool hasMainImage = product.ProductImages.Any(i => i.IsMain); // Kiểm tra đã có ảnh chính chưa

            // Xử lý ảnh mới nếu chưa đạt 5 ảnh
            if (currentImageCount < 5 && newImages?.Any() == true)
            {
                int availableSlots = 5 - currentImageCount;
                var imagesToAdd = new List<Models.ProductImage>();

                foreach (var file in newImages.Take(availableSlots))
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string filePath = Path.Combine(webRootPath, "uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    imagesToAdd.Add(new Models.ProductImage
                    {
                        ImageUrl = "/uploads/" + fileName, // Lưu đường dẫn tương đối
                        ProductID = product.ID,
                        IsMain = !hasMainImage // Đặt IsMain = true nếu chưa có ảnh chính
                    });

                    hasMainImage = true; // Sau khi thêm ảnh đầu tiên, đảm bảo các ảnh sau là IsMain = false
                }

                _context.ProductImages.AddRange(imagesToAdd);
            }

            await _context.SaveChangesAsync();
        }


        public async Task<ProductHideShowViewModel> GetByIdAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            return new ProductHideShowViewModel
            {
                ID = product.ID,
                Name = product.Name,
                IsActive = product.IsActive
            };
        }

        public async Task<bool> UpdateStatusAsync(Guid productId, bool isActive)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            product.IsActive = isActive; // Cập nhật trạng thái
            _context.Products.Update(product);
            await _context.SaveChangesAsync(); // Lưu thay đổi vào DB

            return true;
        }

    }
}
