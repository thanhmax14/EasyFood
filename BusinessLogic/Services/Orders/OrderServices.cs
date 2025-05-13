using System.Linq.Expressions;
using AutoMapper;
using BusinessLogic.Services.BalanceChanges;
using Models;
using Repository.BalanceChange;
using Repository.Categorys;
using Repository.OrdeDetails;
using Repository.OrdersRepository;
using Repository.StoreDetails;
using Repository.ViewModels;

namespace BusinessLogic.Services.Orders
{
    public class OrderServices : IOrdersServices
    {
        private readonly IOrdersRepository _repository;
        private readonly IMapper _mapper;
        private readonly OrdersRepository _orderRepository;
        private readonly IBalanceChangeService _balanceService;
        private readonly BalanceChangeRepository _balanceRepositorys;
        private readonly CategoryRepository _categoryRepositorys;
        private readonly StoreDetailsRepository _atoreDetailRepositorys;
        private readonly OrderDetailRepository _orderDetailRepositorys;

        public OrderServices(IOrdersRepository repository, IMapper mapper, OrdersRepository orderRepository, IBalanceChangeService balanceService, BalanceChangeRepository balanceRepositorys, CategoryRepository categoryRepositorys, StoreDetailsRepository atoreDetailRepositorys)
        {
            _repository = repository;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _balanceService = balanceService;
            _balanceRepositorys = balanceRepositorys;
            _categoryRepositorys = categoryRepositorys;
            _atoreDetailRepositorys = atoreDetailRepositorys;
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
        public async Task<bool> AcceptOrder(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(orderId);
                if (order == null) return false;

                // Lấy OrderDetails trực tiếp từ Order
                var orderDetails = order.OrderDetails;
                if (orderDetails == null || !orderDetails.Any()) return false;

                var productId = orderDetails.First().ProductID;
                var category = await _categoryRepositorys.GetByProductId(productId);
                if (category == null) return false;

                decimal commission = (decimal)category.Commission;
                decimal moneyChange = order.TotalPrice * (commission / 100);

                var store = await _atoreDetailRepositorys.GetByUserId(order.UserID);
                if (store == null) return false;

                decimal moneyBeforeChange = await _balanceService.GetBalance(store.UserID);
                decimal moneyAfterChange = moneyBeforeChange + moneyChange;

                // Cập nhật trạng thái đơn hàng
                order.Status = "Success";
                await _orderRepository.UpdateAsync(order);

                // Thêm bản ghi vào BalanceChanges
                var balanceChange = new BalanceChange
                {
                    ID = Guid.NewGuid(),
                    UserID = store.UserID,
                    MoneyBeforeChange = moneyBeforeChange,
                    MoneyChange = moneyChange,
                    MoneyAfterChange = moneyAfterChange,
                    StartTime = DateTime.UtcNow,
                    Status = "Success",
                    CheckDone = true
                };

                await _balanceRepositorys.AddAsync(balanceChange);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AcceptOrder: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RejectOrder(Guid orderId)
        {
            try
            {
                var order = await _orderRepository.GetOrderByIdAsync(orderId);
                if (order == null) return false;

                decimal moneyBeforeChange = await _balanceService.GetBalance(order.UserID);
                decimal moneyChange = order.TotalPrice;
                decimal moneyAfterChange = moneyBeforeChange + moneyChange;

                // Cập nhật trạng thái order
                order.Status = "CANCELLED";
                await _orderRepository.UpdateAsync(order);

                // Thêm bản ghi vào BalanceChanges
                var balanceChange = new BalanceChange
                {
                    ID = Guid.NewGuid(),
                    UserID = order.UserID,
                    MoneyBeforeChange = moneyBeforeChange,
                    MoneyChange = moneyChange,
                    MoneyAfterChange = moneyAfterChange,
                    StartTime = DateTime.UtcNow,
                    Status = "CANCELLED",
                    CheckDone = true
                };

                await _balanceRepositorys.AddAsync(balanceChange);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RejectOrder: {ex.Message}");
                return false;
            }
        }

    }
}
