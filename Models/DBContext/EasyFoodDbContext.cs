using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DBContext
{
    public class EasyFoodDbContext:IdentityDbContext<AppUser>
    {
 /*       public EasyFoodDbContext(DbContextOptions<EasyFoodDbContext> options) : base(options)
        {
        }
*/
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








        }









        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      => optionsBuilder.UseSqlServer("Server=DESKTOP-1E1A6I4;Database =EasyFood;uid=sa;pwd=Thanh;encrypt=true;trustServerCertificate=true;");
    
}
}
