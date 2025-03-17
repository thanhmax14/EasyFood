using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Models.DBContext
{
    public class EasyFoodDbContext:IdentityDbContext<AppUser>
    {
       /* public EasyFoodDbContext(DbContextOptions<EasyFoodDbContext> options) : base(options)
        {
        }*/

        public DbSet<BalanceChange> BalanceChanges { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<StoreDetails> StoreDetails { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Complain> Complains { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<FavoriteRecipe> FavoriteRecipes { get; set; }
        public DbSet<RecipeReview> RecipeReviews { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        


        
        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            // Bỏ tiền tố AspNet của các bảng: mặc định
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            builder.Entity<StoreDetails>()
            .HasOne(h => h.AppUser) 
            .WithOne(u => u.StoreDetails) 
            .HasForeignKey<StoreDetails>(h => h.UserID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Product>()
           .HasOne(h => h.StoreDetails)
           .WithMany(h => h.Products)
           .HasForeignKey(h => h.StoreID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<BalanceChange>()
           .HasOne(h => h.AppUser)
           .WithMany(h => h.BalanceChanges)
           .HasForeignKey(h => h.UserID).OnDelete(DeleteBehavior.NoAction);


            builder.Entity<Product>()
           .HasOne(h => h.Categories)
           .WithMany(h => h.Products)
           .HasForeignKey(h => h.CateID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProductVariant>()
           .HasOne(h => h.Product)
           .WithMany(h => h.ProductVariants)
           .HasForeignKey(h => h.ProductID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProductImage>()
           .HasOne(h => h.Product)
           .WithMany(h => h.ProductImages)
           .HasForeignKey(h => h.ProductID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Wishlist>()
            .HasOne(h => h.AppUser)
            .WithMany(h => h.Wishlists)
            .HasForeignKey(h => h.UserID).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Wishlist>()
            .HasOne(h => h.Product)
            .WithMany(h => h.Wishlists)
            .HasForeignKey(h => h.ProductID).OnDelete(DeleteBehavior.NoAction); 


            builder.Entity<Cart>()
            .HasOne(h => h.AppUser)
            .WithMany(h => h.Carts)
            .HasForeignKey(h => h.UserID).OnDelete(DeleteBehavior.Cascade); ;

            builder.Entity<Cart>()
            .HasOne(h => h.Product)
            .WithMany(h => h.Carts)
            .HasForeignKey(h => h.ProductID).OnDelete(DeleteBehavior.NoAction); ;


            builder.Entity<Order>()
            .HasOne(h => h.AppUser)
            .WithMany(h => h.Orders)
            .HasForeignKey(h => h.UserID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<OrderDetail>()
            .HasOne(h => h.Product)
            .WithMany(h => h.OrderDetails)
            .HasForeignKey(h => h.ProductID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<OrderDetail>()
           .HasOne(h => h.Order)
           .WithMany(h => h.OrderDetails)
           .HasForeignKey(h => h.OrderID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Complain>()
           .HasOne(h => h.OrderDetail)
           .WithMany(h => h.Complains)
           .HasForeignKey(h => h.OrderDetailID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Recipe>()
           .HasOne(h => h.AppUser)
           .WithMany(h => h.Recipes)
           .HasForeignKey(h => h.UserID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Recipe>()
           .HasOne(h => h.Categories)
           .WithMany(h => h.Recipes)
           .HasForeignKey(h => h.CateID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FavoriteRecipe>()
           .HasOne(h => h.AppUser)
           .WithMany(h => h.FavoriteRecipes)
           .HasForeignKey(h => h.UserID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FavoriteRecipe>()
           .HasOne(h => h.Recipe)
           .WithMany(h => h.FavoriteRecipes)
           .HasForeignKey(h => h.RecipeID).OnDelete(DeleteBehavior.NoAction);  

            builder.Entity<RecipeReview>()
           .HasOne(h => h.Recipe)
           .WithMany(h => h.RecipeReviews)
           .HasForeignKey(h => h.RecipeID).OnDelete(DeleteBehavior.NoAction); 
            
            builder.Entity<FavoriteRecipe>()
           .HasOne(h => h.AppUser)
           .WithMany(h => h.FavoriteRecipes)
           .HasForeignKey(h => h.UserID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Order>()
           .HasOne(h => h.Voucher)
           .WithMany(h => h.Orders)
           .HasForeignKey(h => h.VoucherID).OnDelete(DeleteBehavior.NoAction).IsRequired(false);




        }









        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      => optionsBuilder.UseSqlServer("Server=tcp:easyfood.database.windows.net,1433;Initial Catalog=EasyFood;Persist Security Info=False;User ID=easyfood;Password=Xinchao123@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");


}
}
