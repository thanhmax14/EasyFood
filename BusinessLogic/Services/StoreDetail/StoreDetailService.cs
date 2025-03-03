using AutoMapper;
using Models;
using Repository.Categorys;
using Repository.StoreDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.StoreDetail
{
    public class StoreDetailService : IStoreDetailService
    {
        private readonly IStoreDetailsRepository _repository;
        private readonly IMapper _mapper;

        public StoreDetailService(IStoreDetailsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IQueryable<StoreDetails> GetAll() => _repository.GetAll();

        public StoreDetails GetById(Guid id) => _repository.GetById(id);

        public async Task<StoreDetails> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public StoreDetails Find(Expression<Func<StoreDetails, bool>> match) => _repository.Find(match);

        public async Task<StoreDetails> FindAsync(Expression<Func<StoreDetails, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(StoreDetails entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(StoreDetails entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(StoreDetails entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<StoreDetails>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<StoreDetails>> ListAsync(
            Expression<Func<StoreDetails, bool>> filter = null,
            Func<IQueryable<StoreDetails>, IOrderedQueryable<StoreDetails>> orderBy = null,
            Func<IQueryable<StoreDetails>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StoreDetails, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
}
