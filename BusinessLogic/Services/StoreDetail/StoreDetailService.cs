using AutoMapper;
using Models;
using Repository.Categorys;
using Repository.StoreDetails;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.StoreDetail
{
    public class StoreDetailService : IStoreDetailService
    {
        private readonly IStoreDetailsRepository _repository;
        private readonly StoreDetailsRepository _repositorys;
        private readonly IMapper _mapper;

        public StoreDetailService(IStoreDetailsRepository repository, IMapper mapper, StoreDetailsRepository repositorys)
        {
            _repository = repository;
            _mapper = mapper;
            _repositorys = repositorys;
        }

        public IQueryable<StoreDetails> GetAll() => _repository.GetAll();

        public StoreDetails GetById(Guid id) => _repository.GetById(id);

        public async Task<StoreDetails> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public StoreDetails Find(Expression<Func<StoreDetails, bool>> match) => _repository.Find(match);

        public async Task<StoreDetails> FindAsync(Expression<Func<StoreDetails, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(StoreDetails entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(StoreDetails entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(StoreDetails entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<StoreDetails>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<StoreDetails>> ListAsync(
            Expression<Func<StoreDetails, bool>> filter = null,
            Func<IQueryable<StoreDetails>, IOrderedQueryable<StoreDetails>> orderBy = null,
            Func<IQueryable<StoreDetails>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<StoreDetails, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
        public async Task<bool> AddStoreAsync(StoreDetails store, string userId)
        {
            bool isSeller = await _repositorys.IsUserSellerAsync(userId);
            if (!isSeller)
            {
                return false; // Người dùng không phải Seller
            }

            store.UserID = userId;
            store.CreatedDate = DateTime.Now;
            store.ModifiedDate = null;
            store.Status = "PENDING";
            store.IsActive = false;

            return await _repositorys.AddStoreAsync(store);
        }

        public async Task<IEnumerable<StoreDetails>> GetAllStoresAsync()
        {
            var stores = await _repositorys.GetAllStoresAsync();
            return stores.Where(s => s.Status.ToLower() == "approved" && s.IsActive);
        }

        public async Task<StoreDetails?> GetStoreByIdAsync(Guid storeId)
        {
            return await _repositorys.GetStoreByIdAsync(storeId);
        }
        public async Task<bool> UpdateStoreAsync(Guid id, string name, string longDesc, string shortDesc, string address, string phone, string img)
        {
            var storeDetails = await _repositorys.GetByIdAsync(id);
            if (storeDetails == null) return false;

            storeDetails.Name = name;
            storeDetails.LongDescriptions = longDesc;
            storeDetails.ShortDescriptions = shortDesc;
            storeDetails.Address = address;
            storeDetails.Phone = phone;
            storeDetails.Img = img;
            storeDetails.ModifiedDate = DateTime.Now;

            await _repository.UpdateAsync(storeDetails);
            return true;
        }
        public async Task<List<StoreViewModel>> GetInactiveStoresAsync()
        {
            return await _repositorys.GetInactiveStoresAsync();
        }
        public async Task<bool> HideStoreAsync(Guid storeId)
        {
            var store = await _repositorys.GetByIdAsync(storeId);
            if (store == null)
            {
                return false;
            }

            store.IsActive = false; // Cập nhật trạng thái về 0
            store.ModifiedDate = DateTime.Now;

            await _repository.UpdateAsync(store);
            await _repository.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ShowStoreAsync(Guid storeId)
        {
            var store = await _repositorys.GetByIdAsync(storeId);
            if (store == null)
            {
                return false;
            }

            store.IsActive = true; // Cập nhật trạng thái về 0
            store.ModifiedDate = DateTime.Now;

            await _repository.UpdateAsync(store);
            await _repository.SaveChangesAsync();

            return true;
        }
        public async Task<bool> UpdateStoreAsync(StoreDetails store)
        {
            return await _repositorys.UpdateStoreAsync(store);
        }
        public async Task<List<StoreDetails>> GetStoresAsync()
        {
            return await _repositorys.GetStoresAsync();
        }

        public async Task UpdateStoreStatusAsync(int storeId, bool isActive)
        {
            await _repositorys.UpdateStoreStatusAsync(storeId, isActive);
        }

        public Task<List<StoreViewModel>> GetStoreRegistrationRequestsAsync()
        {
            return _repositorys.GetStoreRegistrationRequestsAsync();
        }
        public async Task<bool> AcceptStoreAsync(Guid id)
        {
            return await _repositorys.AcceptStoreAsync(id);
        }
        public async Task<bool> RejectStoreAsync(Guid id)
        {
            return await _repositorys.RejectStoreAsync(id);
        }

        public async Task<bool> UpdateStoreStatusAsync(Guid storeId, string newStatus)
        {
            var storeDetail = await _repositorys.GetStoreByIdAsync(storeId);
            if (storeDetail == null)
            {
                return false;
            }

            storeDetail.Status = newStatus;
            storeDetail.ModifiedDate = DateTime.UtcNow;

            await _repositorys.UpdateStoreAsync(storeDetail);
            return true;
        }
    }
}
