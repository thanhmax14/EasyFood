using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.StoreDetail
{
    public interface IStoreDetailService
    {
        IQueryable<StoreDetails> GetAll();
        StoreDetails GetById(Guid id);
        Task<StoreDetails> GetAsyncById(Guid id);
        StoreDetails Find(Expression<Func<StoreDetails, bool>> match);
        Task<StoreDetails> FindAsync(Expression<Func<StoreDetails, bool>> match);
        Task AddAsync(StoreDetails entity);
        Task UpdateAsync(StoreDetails entity);
        Task DeleteAsync(StoreDetails entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<StoreDetails>> ListAsync();
        Task<IEnumerable<StoreDetails>> ListAsync(
            Expression<Func<StoreDetails, bool>> filter = null,
            Func<IQueryable<StoreDetails>, IOrderedQueryable<StoreDetails>> orderBy = null,
            Func<IQueryable<StoreDetails>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StoreDetails, object>> includeProperties = null);
    }
}
