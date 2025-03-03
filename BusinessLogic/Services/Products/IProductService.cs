using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Products
{
    public interface IProductService
    {
        IQueryable<Product> GetAll();
        Product GetById(Guid id);
        Task<Product> GetAsyncById(Guid id);
        Product Find(Expression<Func<Product, bool>> match);
        Task<Product> FindAsync(Expression<Func<Product, bool>> match);
        Task AddAsync(Product entity);
        Task UpdateAsync(Product entity);
        Task DeleteAsync(Product entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<Product>> ListAsync();
        Task<IEnumerable<Product>> ListAsync(
            Expression<Func<Product, bool>> filter = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null,
            Func<IQueryable<Product>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Product, object>> includeProperties = null);
    }
}
