using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.ProductImages
{
    public interface IProductImageService
    {
        IQueryable<ProductImage> GetAll();
        ProductImage GetById(Guid id);
        Task<ProductImage> GetAsyncById(Guid id);
        ProductImage Find(Expression<Func<ProductImage, bool>> match);
        Task<ProductImage> FindAsync(Expression<Func<ProductImage, bool>> match);
        Task AddAsync(ProductImage entity);
        Task UpdateAsync(ProductImage entity);
        Task DeleteAsync(ProductImage entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<ProductImage>> ListAsync();
        Task<IEnumerable<ProductImage>> ListAsync(
            Expression<Func<ProductImage, bool>> filter = null,
            Func<IQueryable<ProductImage>, IOrderedQueryable<ProductImage>> orderBy = null,
            Func<IQueryable<ProductImage>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductImage, object>> includeProperties = null);
    }
}
