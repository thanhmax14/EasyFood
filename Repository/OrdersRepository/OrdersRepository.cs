using Microsoft.EntityFrameworkCore;
using Models;
using Models.DBContext;
using Repository.BaseRepository;

namespace Repository.OrdersRepository
{
    public class OrdersRepository : BaseRepository<Models.Order>, IOrdersRepository
    {
        private readonly EasyFoodDbContext _context;
        public OrdersRepository(EasyFoodDbContext context) : base(context) {
            _context = context;
        }
        public async Task<Order> GetOrderByIdAsync(Guid orderId)
        {
            return await _context.Orders
                                 .Include(o => o.OrderDetails)
                                 .ThenInclude(od => od.Product)
                                 .ThenInclude(p => p.Categories)
                                 .FirstOrDefaultAsync(o => o.ID == orderId) ?? throw new Exception("Order not found");
        }


        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task AddBalanceChangeAsync(Models.BalanceChange balanceChange)
        {
            _context.BalanceChanges.Add(balanceChange);
            await _context.SaveChangesAsync();
        }


    }

}
