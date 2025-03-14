using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Wishlists
{
    public interface IWishlistServices
    {
        IQueryable<Wishlist> GetAll();
        Wishlist GetById(Guid id);
        Task<Wishlist> GetAsyncById(Guid id);
        Wishlist Find(Expression<Func<Wishlist, bool>> match);
        Task<Wishlist> FindAsync(Expression<Func<Wishlist, bool>> match);
        Task AddAsync(Wishlist entity);
        Task UpdateAsync(Wishlist entity);
        Task DeleteAsync(Wishlist entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<Wishlist>> ListAsync();
        Task<IEnumerable<Wishlist>> ListAsync(
            Expression<Func<Wishlist, bool>> filter = null,
            Func<IQueryable<Wishlist>, IOrderedQueryable<Wishlist>> orderBy = null,
            Func<IQueryable<Wishlist>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Wishlist, object>> includeProperties = null);
    }
}
