using Models;
using Models.DBContext;
using Repository.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.OrdeDetails
{
    public class OrderDetailRepository:BaseRepository<OrderDetail>, IOrderDetailRepository  
    {
        public OrderDetailRepository(EasyFoodDbContext context) : base(context)
        {
        }
    }
}
