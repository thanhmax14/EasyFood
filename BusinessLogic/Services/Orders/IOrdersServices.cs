using System.Linq.Expressions;
using Models;

namespace BusinessLogic.Services.Orders
{
    public interface IOrdersServices
    {
        IQueryable<Order> GetAll();
        Order GetById(Guid id);
        Task<Order> GetAsyncById(Guid id);
        Order Find(Expression<Func<Order, bool>> match);
        Task<Order> FindAsync(Expression<Func<Order, bool>> match);
        Task AddAsync(Order entity);
        Task UpdateAsync(Order entity);
        Task DeleteAsync(Order entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<Order>> ListAsync();
        Task<IEnumerable<Order>> ListAsync(
            Expression<Func<Order, bool>> filter = null,
            Func<IQueryable<Order>, IOrderedQueryable<Order>> orderBy = null,
            Func<IQueryable<Order>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Order, object>> includeProperties = null);
    }
}
