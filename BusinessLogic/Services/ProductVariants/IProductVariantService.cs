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
        IQueryable<ProductTypes> GetAll();
        ProductTypes GetById(Guid id);
        Task<ProductTypes> GetAsyncById(Guid id);
        ProductTypes Find(Expression<Func<ProductTypes, bool>> match);
        Task<ProductTypes> FindAsync(Expression<Func<ProductTypes, bool>> match);
        Task AddAsync(ProductTypes entity);
        Task UpdateAsync(ProductTypes entity);
        Task DeleteAsync(ProductTypes entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<ProductTypes>> ListAsync();
        Task<IEnumerable<ProductTypes>> ListAsync(
            Expression<Func<ProductTypes, bool>> filter = null,
            Func<IQueryable<ProductTypes>, IOrderedQueryable<ProductTypes>> orderBy = null,
            Func<IQueryable<ProductTypes>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<ProductTypes, object>> includeProperties = null);
        Task<List<ProductVariantViewModel>> GetVariantsByProductIdAsync(Guid productId);
        Task CreateProductVariantAsync(ProductVariantCreateViewModel model);
        Task<bool> UpdateProductVariantAsync(ProductVariantEditViewModel model);
        Task<ProductVariantEditViewModel> GetProductVariantForEditAsync(Guid variantId);
        bool UpdateProductVariantStatus(Guid variantId, bool isActive);
    }
}
