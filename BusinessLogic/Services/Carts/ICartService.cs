using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Carts
{
    public interface ICartService
    {
        IQueryable<Cart> GetAll();
        Cart GetById(Guid id);
        Task<Cart> GetAsyncById(Guid id);
        Cart Find(Expression<Func<Cart, bool>> match);
        Task<Cart> FindAsync(Expression<Func<Cart, bool>> match);
        Task AddAsync(Cart entity);
        Task UpdateAsync(Cart entity);
        Task DeleteAsync(Cart entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<Cart>> ListAsync();
        Task<IEnumerable<Cart>> ListAsync(
            Expression<Func<Cart, bool>> filter = null,
            Func<IQueryable<Cart>, IOrderedQueryable<Cart>> orderBy = null,
            Func<IQueryable<Cart>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Cart, object>> includeProperties = null);
    }
}
