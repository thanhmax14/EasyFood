using Models.DBContext;
using Models;
using Repository.BaseRepository;
using Repository.Categorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repository.StoreDetails
{
    public class StoreDetailsRepository : BaseRepository<Models.StoreDetails>, IStoreDetailsRepository
    {
        private readonly EasyFoodDbContext _context;
        public StoreDetailsRepository(EasyFoodDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> IsUserSellerAsync(string userId)
        {
            return await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId &&
                                _context.Roles.Any(r => r.Id == ur.RoleId && r.Name.ToLower() == "seller"));
        }

        public async Task<bool> AddStoreAsync(Models.StoreDetails store)
        {
            try
            {
                await _context.StoreDetails.AddAsync(store);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
