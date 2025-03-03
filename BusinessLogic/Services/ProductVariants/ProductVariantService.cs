using AutoMapper;
using BusinessLogic.Services.ProductVariants;
using Models;
using Repository.ProductVariants;
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

        public ProductVariantService(IProductVariantRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IQueryable<ProductVariant> GetAll() => _repository.GetAll();

        public ProductVariant GetById(Guid id) => _repository.GetById(id);

        public async Task<ProductVariant> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public ProductVariant Find(Expression<Func<ProductVariant, bool>> match) => _repository.Find(match);

        public async Task<ProductVariant> FindAsync(Expression<Func<ProductVariant, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(ProductVariant entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(ProductVariant entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(ProductVariant entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<ProductVariant>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<ProductVariant>> ListAsync(
            Expression<Func<ProductVariant, bool>> filter = null,
            Func<IQueryable<ProductVariant>, IOrderedQueryable<ProductVariant>> orderBy = null,
            Func<IQueryable<ProductVariant>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductVariant, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
}
