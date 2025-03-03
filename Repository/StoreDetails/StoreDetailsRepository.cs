using Models.DBContext;
using Models;
using Repository.BaseRepository;
using Repository.Categorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.StoreDetails
{
    public class StoreDetailsRepository : BaseRepository<Models.StoreDetails>, IStoreDetailsRepository
    {
        public StoreDetailsRepository(EasyFoodDbContext context) : base(context) { }
    }
}
