using Models.DBContext;
using Repository.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Carts
{
    public class CartRepository : BaseRepository<Models.Cart>, ICartRepository
    {
        public CartRepository(EasyFoodDbContext context) : base(context) { }
    }
}
