using AutoMapper;
using BusinessLogic.Services.ProductVariants;
using Models;
using Repository.ProductVariants;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.ProductVariantVariants
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _repository;
        private readonly IMapper _mapper;
        private readonly ProductVariantRepository _repositorys;

        public ProductVariantService(IProductVariantRepository repository, IMapper mapper, ProductVariantRepository repositorys)
        {
            _repository = repository;
            _mapper = mapper;
            _repositorys = repositorys;
        }

        public IQueryable<ProductTypes> GetAll() => _repository.GetAll();

        public ProductTypes GetById(Guid id) => _repository.GetById(id);

        public async Task<ProductTypes> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public ProductTypes Find(Expression<Func<ProductTypes, bool>> match) => _repository.Find(match);

        public async Task<ProductTypes> FindAsync(Expression<Func<ProductTypes, bool>> match) => await _repository.FindAsync(match);
      
        public async Task AddAsync(ProductTypes entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(ProductTypes entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(ProductTypes entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<ProductTypes>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<ProductTypes>> ListAsync(
            Expression<Func<ProductTypes, bool>> filter = null,
            Func<IQueryable<ProductTypes>, IOrderedQueryable<ProductTypes>> orderBy = null,
            Func<IQueryable<ProductTypes>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductTypes, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
        public async Task<List<ProductVariantViewModel>> GetVariantsByProductIdAsync(Guid productId)
        {
            return await _repositorys.GetVariantsByProductIdAsync(productId);
        }
        public async Task CreateProductVariantAsync(ProductVariantCreateViewModel model)
        {
            await _repositorys.CreateProductVariantAsync(model);
        }
        public async Task<ProductVariantEditViewModel> GetProductVariantForEditAsync(Guid variantId)
        {
            return await _repositorys.GetProductVariantForEditAsync(variantId);
        }

        public async Task<bool> UpdateProductVariantAsync(ProductVariantEditViewModel model)
        {
            return await _repositorys.UpdateProductVariantAsync(model);
        }

        public bool UpdateProductVariantStatus(Guid variantId, bool isActive)
        {
            return _repositorys.UpdateProductVariantStatus(variantId, isActive);
        }
    }
}
