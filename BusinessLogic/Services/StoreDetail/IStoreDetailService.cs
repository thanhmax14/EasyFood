using Models;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.StoreDetail
{
    public interface IStoreDetailService
    {
        IQueryable<StoreDetails> GetAll();
        StoreDetails GetById(Guid id);
        Task<StoreDetails> GetAsyncById(Guid id);
        StoreDetails Find(Expression<Func<StoreDetails, bool>> match);
        Task<StoreDetails> FindAsync(Expression<Func<StoreDetails, bool>> match);
        Task AddAsync(StoreDetails entity);
        Task UpdateAsync(StoreDetails entity);
        Task DeleteAsync(StoreDetails entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> SaveChangesAsync();
        int Count();
        Task<int> CountAsync();
        Task<IEnumerable<StoreDetails>> ListAsync();
        Task<IEnumerable<StoreDetails>> ListAsync(
            Expression<Func<StoreDetails, bool>> filter = null,
            Func<IQueryable<StoreDetails>, IOrderedQueryable<StoreDetails>> orderBy = null,
            Func<IQueryable<StoreDetails>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StoreDetails, object>> includeProperties = null);
        Task<bool> AddStoreAsync(StoreDetails store, string userId);
        Task<IEnumerable<StoreDetails>> GetAllStoresAsync();
        Task<StoreDetails?> GetStoreByIdAsync(Guid storeId);
        Task<bool> UpdateStoreAsync(Guid id, string name, string longDesc, string shortDesc, string address, string phone, string img);
        Task<List<StoreViewModel>> GetInactiveStoresAsync();
        Task<bool> HideStoreAsync(Guid storeId);
        Task<bool> UpdateStoreAsync(StoreDetails store);
        Task<bool> ShowStoreAsync(Guid storeId);
        Task<List<StoreDetails>> GetStoresAsync();
        Task UpdateStoreStatusAsync(int storeId, bool isActive);
        Task<List<StoreViewModel>> GetStoreRegistrationRequestsAsync();
        Task<bool> AcceptStoreAsync(Guid id);
        Task<bool> RejectStoreAsync(Guid id);
        Task<bool> UpdateStoreStatusAsync(Guid storeId, string newStatus);
        Task<IEnumerable<StoreViewModel>> GetStoresByUserIdAsync(string? userId);
    }
}
