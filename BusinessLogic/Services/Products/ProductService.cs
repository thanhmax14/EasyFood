using AutoMapper;
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

        public ProductService(IProductsRepository repository, IMapper mapper, CategoryRepository categoryRepository, ProductImageRepository productImageRepository, ProductsRepository repositorys)
        {
            _repository = repository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _productImageRepository = productImageRepository;
            _repositorys = repositorys;
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
                CateID = model.CateID,
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

    }
}
