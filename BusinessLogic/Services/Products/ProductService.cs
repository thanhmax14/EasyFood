using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Models;
using Repository.Categorys;
using Repository.ProductImage;
using Repository.Products;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductsRepository _repository;
        private readonly IMapper _mapper;
        private readonly CategoryRepository _categoryRepository;
        private readonly ProductImageRepository _productImageRepository;
        private readonly ProductsRepository _repositorys;
        private readonly IWebHostEnvironment _env;

        public ProductService(IProductsRepository repository, IMapper mapper, CategoryRepository categoryRepository, ProductImageRepository productImageRepository, ProductsRepository repositorys, IWebHostEnvironment env)
        {
            _repository = repository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _productImageRepository = productImageRepository;
            _repositorys = repositorys;
            _env = env;
        }

        public IQueryable<Product> GetAll() => _repository.GetAll();

        public Product GetById(Guid id) => _repository.GetById(id);

        public async Task<Product> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public Product Find(Expression<Func<Product, bool>> match) => _repository.Find(match);

        public async Task<Product> FindAsync(Expression<Func<Product, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(Product entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(Product entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(Product entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<Product>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<Product>> ListAsync(
            Expression<Func<Product, bool>> filter = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null,
            Func<IQueryable<Product>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Product, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
        public async Task<bool> CreateProductAsync(ProductListViewModel model, string userId, List<ProductImageViewModel> images)
        {
            var storeId = await GetCurrentStoreIDAsync(userId);
            if (storeId == null)
                return false;

            var product = new Product
            {
                ID = Guid.NewGuid(),
                Name = model.Name,
                ShortDescription = model.ShortDescription,
                LongDescription = model.LongDescription,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = null,
                ManufactureDate = model.ManufactureDate,
                IsActive = true,
                IsOnSale = false,
                CategoryID = model.CateID,
                StoreID = storeId
            };
            var result = await _repositorys.AddAsync(product);

            if (!result)
                return false;

            var productImages = images.Select((img, index) => new ProductImage
            {
                ID = Guid.NewGuid(),
                ProductID = product.ID,
                ImageUrl = img.ImageUrl,
                IsMain = index == 0 // Hình đầu tiên là hình chính
            }).ToList();

            return await _productImageRepository.AddRangeAsync(productImages);
        }

        public Task<Guid> GetCurrentStoreIDAsync(string userId)
        {
            return _repositorys.GetCurrentStoreIDAsync(userId);
        }

        public async Task<List<ProductListViewModel>> GetAllProductsAsync(Guid storeId)
        {
            return await _repositorys.GetProductsWithDetailsByStoreIdAsync(storeId);
        }
        public List<ProductIndexViewModel> GetProductsByStoreId(Guid storeId)
        {
            return _repositorys.GetProductsByStoreId(storeId);
        }
        public async Task<Guid> CreateProductAsync(ProductViewModel model)
        {
            var product = new Product
            {
                ID = Guid.NewGuid(),
                Name = model.Name.Trim(),
                ShortDescription = model.ShortDescription.Trim(),
                LongDescription = model.LongDescription.Trim(),
                CreatedDate = DateTime.UtcNow,
                ManufactureDate = model.ManufactureDate,
                IsActive = model.IsActive,
                IsOnSale = model.IsOnSale,
                CategoryID = model.CateID,
                StoreID = model.StoreID
            };

            List<ProductImage> images = new List<ProductImage>();
            if (model.Images != null && model.Images.Count > 0)
            {
                string uploadPath = Path.Combine(_env.WebRootPath, "uploads");

                for (int i = 0; i < model.Images.Count && i < 5; i++)
                {
                    var image = model.Images[i];
                    string fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                    string filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    images.Add(new ProductImage
                    {
                        ID = Guid.NewGuid(),
                        ImageUrl = $"/uploads/{fileName}",
                        IsMain = (i == 0), // Hình đầu tiên là IsMain
                        ProductID = product.ID
                    });
                }
            }

            return await _repositorys.CreateProductAsync(product, images);
        }

        public async Task<List<Categories>> GetCategoriesAsync()
        {
            return await _repositorys.GetCategoriesAsync();
        }

        public async Task<ProductUpdateViewModel> GetProductByIdAsync(Guid productId)
        {
            return await _repositorys.GetProductByIdAsync(productId);
        }

        public async Task UpdateProductAsync(ProductUpdateViewModel model, List<IFormFile> newImages, string webRootPath)
        {
            await _repositorys.UpdateProductAsync(model, newImages, webRootPath);
        }
        public async Task<bool> ToggleProductStatus(Guid productId)
        {
            var product = await _repositorys.GetByIdAsync(productId);
            if (product == null) return false;

            bool newStatus = !product.IsActive; // Đảo trạng thái Hide/Show
            return await _repositorys.UpdateStatusAsync(productId, newStatus);
        }

        public List<ProductIndexViewModel> GetProductsByCurrentUser(string userId)
        {
            return  _repositorys.GetProductsByCurrentUser(userId);
        }
    }
}
