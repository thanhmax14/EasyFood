using Models;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.Categorys
{
    public interface ICategoryService
    {
        IQueryable<Categories> GetAll();
        Categories GetById(Guid id);
        Task<Categories> GetAsyncById(Guid id);
        Categories Find(Expression<Func<Categories, bool>> match);
        Task<Categories> FindAsync(Expression<Func<Categories, bool>> match);
        Task AddAsync(Categories entity);
        Task UpdateAsync(Categories entity);
        Task DeleteAsync(Categories entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<Categories>> ListAsync();
        Task<IEnumerable<Categories>> ListAsync(
            Expression<Func<Categories, bool>> filter = null,
            Func<IQueryable<Categories>, IOrderedQueryable<Categories>> orderBy = null,
            Func<IQueryable<Categories>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Categories, object>> includeProperties = null);
        Task<IEnumerable<CategoryViewModel>> GetAllAsync();

    }
}
