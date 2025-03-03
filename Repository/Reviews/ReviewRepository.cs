using Models.DBContext;
using Repository.BaseRepository;
using Repository.Reviews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Reviews
{
    public class ReviewRepository : BaseRepository<Models.Review>, IReviewRepository
    {
        public ReviewRepository(EasyFoodDbContext context) : base(context) { }
    }
}
