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
        public EasyFoodDbContext(DbContextOptions<EasyFoodDbContext> options) : base(options)
        {
        }



        public DbSet<BalanceChange> BalanceChanges { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<StoreDetails> StoreDetails { get; set; }
        
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
            .HasForeignKey<StoreDetails>(h => h.UserID);

            builder.Entity<Product>()
           .HasOne(h => h.StoreDetails)
           .WithMany(h => h.Products)
           .HasForeignKey(h => h.StoreID);

            builder.Entity<BalanceChange>()
           .HasOne(h => h.AppUser)
           .WithMany(h => h.BalanceChanges)
           .HasForeignKey(h => h.UserID);


            builder.Entity<Product>()
           .HasOne(h => h.Categories)
           .WithMany(h => h.Products)
           .HasForeignKey(h => h.CateID);

            builder.Entity<ProductVariant>()
           .HasOne(h => h.Product)
           .WithMany(h => h.ProductVariants)
           .HasForeignKey(h => h.ProductID);

            builder.Entity<ProductImage>()
           .HasOne(h => h.Product)
           .WithMany(h => h.ProductImages)
           .HasForeignKey(h => h.ProductID);





        }









        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      => optionsBuilder.UseSqlServer("Data Source=SQL1002.site4now.net;Initial Catalog=db_ab376a_easyfood;User Id=db_ab376a_easyfood_admin;Password=Xinchao123@");


}
}
