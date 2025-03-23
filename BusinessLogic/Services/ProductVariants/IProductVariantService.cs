using Models;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.ProductVariants
{
    public interface IProductVariantService
    {
        IQueryable<ProductVariant> GetAll();
        ProductVariant GetById(Guid id);
        Task<ProductVariant> GetAsyncById(Guid id);
        ProductVariant Find(Expression<Func<ProductVariant, bool>> match);
        Task<ProductVariant> FindAsync(Expression<Func<ProductVariant, bool>> match);
        Task AddAsync(ProductVariant entity);
        Task UpdateAsync(ProductVariant entity);
        Task DeleteAsync(ProductVariant entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<ProductVariant>> ListAsync();
        Task<IEnumerable<ProductVariant>> ListAsync(
            Expression<Func<ProductVariant, bool>> filter = null,
            Func<IQueryable<ProductVariant>, IOrderedQueryable<ProductVariant>> orderBy = null,
            Func<IQueryable<ProductVariant>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductVariant, object>> includeProperties = null);
        Task<List<ProductVariantViewModel>> GetVariantsByProductIdAsync(Guid productId);
        Task CreateProductVariantAsync(ProductVariantCreateViewModel model);
        Task<bool> UpdateProductVariantAsync(ProductVariantEditViewModel model);
        Task<ProductVariantEditViewModel> GetProductVariantForEditAsync(Guid variantId);
        bool UpdateProductVariantStatus(Guid variantId, bool isActive);
    }
}
