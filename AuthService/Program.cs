using Microsoft.AspNetCore.Identity;
using Models.DBContext;
using Models;
using BusinessLogic.Config;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<EasyFoodDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register default identity using ThanhMMODbContext for Identity stores
builder.Services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<EasyFoodDbContext>().AddDefaultTokenProviders(); ;
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.ConfigureServices();
builder.Services.ConfigureRepository();
// Register Repository layer
builder.Services.AddScoped<Repository.StoreDetails.StoreDetailsRepository>();
builder.Services.AddScoped<Repository.Categorys.ICategoryRepository, Repository.Categorys.CategoryRepository>();
builder.Services.AddScoped<Repository.Products.IProductsRepository, Repository.Products.ProductsRepository>();
builder.Services.AddScoped<Repository.Categorys.CategoryRepository>();
builder.Services.AddScoped<Repository.Products.ProductsRepository>();
builder.Services.AddScoped<Repository.ProductImage.ProductImageRepository>();
builder.Services.AddScoped<Repository.ProductImage.IProductImageRepository, Repository.ProductImage.ProductImageRepository>();

// Register Service layer
builder.Services.AddScoped<BusinessLogic.Services.Categorys.ICategoryService, BusinessLogic.Services.Categorys.CategoryService>();
builder.Services.AddScoped<BusinessLogic.Services.Products.IProductService, BusinessLogic.Services.Products.ProductService>();
builder.Services.AddScoped<BusinessLogic.Services.StoreDetail.IStoreDetailService, BusinessLogic.Services.StoreDetail.StoreDetailService>();
builder.Services.AddScoped<BusinessLogic.Services.Categorys.CategoryService>();
builder.Services.AddScoped<BusinessLogic.Services.Products.ProductService>();
builder.Services.AddHttpClient();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.Configure<IdentityOptions>(options =>
{

    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    //setting for user
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;

});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
await SeedDataAsync(app);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllers();

app.Run();
static async Task SeedDataAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();

        // Create roles if they don't exist
        string[] roles = { "User", "Admin", "Seller" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }


        // Create a default User user if it doesn't exist
        var User = await userManager.FindByEmailAsync("thanhpqce171732@fpt.edu.vn");
        if (User == null)
        {
            User = new AppUser { UserName = "thanhmax14", Email = "thanhpqce171732@fpt.edu.vn" };
            var result = await userManager.CreateAsync(User, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(User, "User");
            }
        }



        // Create a default admin user if it doesn't exist
        var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
        if (adminUser == null)
        {
            adminUser = new AppUser { UserName = "admin", Email = "admin@gmail.com" };
            var result = await userManager.CreateAsync(adminUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Create a default CTV user if it doesn't exist
        var ctvUser = await userManager.FindByEmailAsync("ctv@gmail.com");
        if (ctvUser == null)
        {
            ctvUser = new AppUser { UserName = "ctv", Email = "ctv@gmail.com" };
            var result = await userManager.CreateAsync(ctvUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(ctvUser, "Seller");
            }
        }
    }
}