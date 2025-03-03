using Models.DBContext;
using Repository.BaseRepository;
using Repository.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.BalanceChange
{
    public class BalanceChangeRepository : BaseRepository<Models.BalanceChange>, IBalanceChangeRepository
    {
        public BalanceChangeRepository(EasyFoodDbContext context) : base(context) { }
    }
}
