using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.BalanceChanges
{
    public interface IBalanceChangeService
    {
        IQueryable<BalanceChange> GetAll();
        BalanceChange GetById(Guid id);
        Task<BalanceChange> GetAsyncById(Guid id);
        BalanceChange Find(Expression<Func<BalanceChange, bool>> match);
        Task<BalanceChange> FindAsync(Expression<Func<BalanceChange, bool>> match);
        Task AddAsync(BalanceChange entity);
        Task UpdateAsync(BalanceChange entity);
        Task DeleteAsync(BalanceChange entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<BalanceChange>> ListAsync();
        Task<IEnumerable<BalanceChange>> ListAsync(
            Expression<Func<BalanceChange, bool>> filter = null,
            Func<IQueryable<BalanceChange>, IOrderedQueryable<BalanceChange>> orderBy = null,
            Func<IQueryable<BalanceChange>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<BalanceChange, object>> includeProperties = null);

        Task<decimal> GetBalance(string UserId);
        Task<bool>CheckMoney(string userID, decimal Money);
    }
}
