using AutoMapper;
using Models;
using Repository.BalanceChange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.BalanceChanges
{
    public class BalanceChangeService : IBalanceChangeService
    {
        private readonly IBalanceChangeRepository _repository;
        private readonly IMapper _mapper;

        public BalanceChangeService(IBalanceChangeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IQueryable<BalanceChange> GetAll() => _repository.GetAll();

        public BalanceChange GetById(Guid id) => _repository.GetById(id);

        public async Task<BalanceChange> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public BalanceChange Find(Expression<Func<BalanceChange, bool>> match) => _repository.Find(match);

        public async Task<BalanceChange> FindAsync(Expression<Func<BalanceChange, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(BalanceChange entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(BalanceChange entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(BalanceChange entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<BalanceChange>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<BalanceChange>> ListAsync(
            Expression<Func<BalanceChange, bool>> filter = null,
            Func<IQueryable<BalanceChange>, IOrderedQueryable<BalanceChange>> orderBy = null,
            Func<IQueryable<BalanceChange>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<BalanceChange, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
}
