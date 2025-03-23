using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AppUser:IdentityUser
    {
        public DateTime? joinin { get; set; } = DateTime.Now;
        public DateTime lastAssces { get; set; } = DateTime.Now;
        public string? FirstName { get; set; } = default;
        public string? LastName  { get; set; } = default;
        public DateTime? Birthday { get; set; }
        public string? Address { get; set; } = default;
        public string? RequestSeller { get; set; } = "0";
        public string? img { get; set; } = "~/assets/imgs/theme/icons/icon-user.svg";
        public bool isUpdateProfile { get; set; } = false;
        public bool IsBanByadmin { get; set; } = false;
        public DateTime? ModifyUpdate { get; set; } = DateTime.Now;



        public virtual StoreDetails StoreDetails { get; set; }
        public ICollection<BalanceChange> BalanceChanges { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<FavoriteRecipe> FavoriteRecipes { get; set; }
        public ICollection<Recipe> Recipes { get; set; }

    }
}
