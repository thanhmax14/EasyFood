using Models;
using Models.DBContext;
using Repository.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Wishlists
{
    public class WishlistRepository:BaseRepository<Wishlist>,IWishlistRepository
    {
        public WishlistRepository(EasyFoodDbContext context):base(context) { }
        
    }
}
