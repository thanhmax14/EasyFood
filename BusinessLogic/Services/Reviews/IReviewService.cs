using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Reviews
{
    public interface IReviewService
    {
        IQueryable<Review> GetAll();
        Review GetById(Guid id);
        Task<Review> GetAsyncById(Guid id);
        Review Find(Expression<Func<Review, bool>> match);
        Task<Review> FindAsync(Expression<Func<Review, bool>> match);
        Task AddAsync(Review entity);
        Task UpdateAsync(Review entity);
        Task DeleteAsync(Review entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<Review>> ListAsync();
        Task<IEnumerable<Review>> ListAsync(
            Expression<Func<Review, bool>> filter = null,
            Func<IQueryable<Review>, IOrderedQueryable<Review>> orderBy = null,
            Func<IQueryable<Review>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Review, object>> includeProperties = null);
    }
}
