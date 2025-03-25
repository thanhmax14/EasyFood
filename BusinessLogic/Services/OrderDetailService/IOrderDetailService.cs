using Models;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.OrderDetailService
{
    public interface IOrderDetailService
    {
        IQueryable<OrderDetail> GetAll();
        OrderDetail GetById(Guid id);
        Task<OrderDetail> GetAsyncById(Guid id);
        OrderDetail Find(Expression<Func<OrderDetail, bool>> match);
        Task<OrderDetail> FindAsync(Expression<Func<OrderDetail, bool>> match);
        Task AddAsync(OrderDetail entity);
        Task UpdateAsync(OrderDetail entity);
        Task DeleteAsync(OrderDetail entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<OrderDetail>> ListAsync();
        Task<IEnumerable<OrderDetail>> ListAsync(
            Expression<Func<OrderDetail, bool>> filter = null,
            Func<IQueryable<OrderDetail>, IOrderedQueryable<OrderDetail>> orderBy = null,
            Func<IQueryable<OrderDetail>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDetail, object>> includeProperties = null);
        Task<List<OrderDetailSellerViewModel>> GetOrderDetailsByOrderIdAsync(Guid storeId);
    }
}
