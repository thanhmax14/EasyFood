using Models.DBContext;
using Repository.BaseRepository;

namespace Repository.OrdersRepository
{
    public class OrdersRepository : BaseRepository<Models.Order>, IOrdersRepository
    {
        public OrdersRepository(EasyFoodDbContext context) : base(context) { }
    }

}
