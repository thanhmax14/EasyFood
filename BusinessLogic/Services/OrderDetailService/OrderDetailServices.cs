using AutoMapper;
using Models;
using Repository.OrdeDetails;
using Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Services.OrderDetailService
{
    public class OrderDetailServices:IOrderDetailService
    {
        private readonly IOrderDetailRepository _repository;
        private readonly IMapper _mapper;
        private readonly OrderDetailRepository _repositorys;

        public OrderDetailServices(IOrderDetailRepository repository, IMapper mapper, OrderDetailRepository repositorys)
        {
            _repository = repository;
            _mapper = mapper;
            _repositorys = repositorys;
        }

        public IQueryable<OrderDetail> GetAll() => _repository.GetAll();

        public OrderDetail GetById(Guid id) => _repository.GetById(id);

        public async Task<OrderDetail> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public OrderDetail Find(Expression<Func<OrderDetail, bool>> match) => _repository.Find(match);

        public async Task<OrderDetail> FindAsync(Expression<Func<OrderDetail, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(OrderDetail entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(OrderDetail entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(OrderDetail entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<OrderDetail>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<OrderDetail>> ListAsync(
            Expression<Func<OrderDetail, bool>> filter = null,
            Func<IQueryable<OrderDetail>, IOrderedQueryable<OrderDetail>> orderBy = null,
            Func<IQueryable<OrderDetail>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<OrderDetail, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
        public async Task<List<OrderDetailSellerViewModel>> GetOrderDetailsByOrderIdAsync(Guid storeId)
        {
            return await _repositorys.GetOrderDetailsByOrderIdAsync(storeId);
        }

    }
}
