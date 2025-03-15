using System.Linq.Expressions;
using AutoMapper;
using Models;
using Repository.OrdersRepository;

namespace BusinessLogic.Services.Orders
{
    public class OrderServices : IOrdersServices
    {
        private readonly IOrdersRepository _repository;
        private readonly IMapper _mapper;

        public OrderServices(IOrdersRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IQueryable<Order> GetAll() => _repository.GetAll();

        public Order GetById(Guid id) => _repository.GetById(id);

        public async Task<Order> GetAsyncById(Guid id) => await _repository.GetAsyncById(id);

        public Order Find(Expression<Func<Order, bool>> match) => _repository.Find(match);

        public async Task<Order> FindAsync(Expression<Func<Order, bool>> match) => await _repository.FindAsync(match);

        public async Task AddAsync(Order entity) => await _repository.AddAsync(entity);

        public async Task UpdateAsync(Order entity) => await _repository.UpdateAsync(entity);

        public async Task DeleteAsync(Order entity) => await _repository.DeleteAsync(entity);

        public async Task DeleteAsync(Guid id) => await _repository.DeleteAsync(id);

        public async Task<bool> ExistsAsync(Guid id) => await _repository.ExistsAsync(id);
        public int Count() => _repository.Count();

        public async Task<int> CountAsync() => await _repository.CountAsync();

        public async Task<IEnumerable<Order>> ListAsync() => await _repository.ListAsync();

        public async Task<IEnumerable<Order>> ListAsync(
            Expression<Func<Order, bool>> filter = null,
            Func<IQueryable<Order>, IOrderedQueryable<Order>> orderBy = null,
            Func<IQueryable<Order>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Order, object>> includeProperties = null) =>
            await _repository.ListAsync(filter, orderBy, includeProperties);
        public async Task<int> SaveChangesAsync() => await _repository.SaveChangesAsync();
    }
}
