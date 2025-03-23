using AutoMapper;
using BusinessLogic.Mapper;
using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Carts;
using BusinessLogic.Services.Categorys;
using Microsoft.AspNetCore.Identity.UI.Services;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.Products;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.ProductVariantVariants;
using BusinessLogic.Services.Reviews;
using BusinessLogic.Services.StoreDetail;
using Microsoft.Extensions.DependencyInjection;
using Net.payOS;
using Repository.StoreDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessLogic.EmailServices.EmailService;
using BusinessLogic.Services.Wishlists;
using BusinessLogic.Hash;
using Repository.BalanceChange;
using BusinessLogic.Services.Orders;

namespace BusinessLogic.Config
{
    public static class CustomServices
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IStoreDetailService, StoreDetailService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBalanceChangeService, BalanceChangeService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IProductVariantService, ProductVariantService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IWishlistServices, WishlistServices>();
            services.AddScoped<ManageTransaction>();
            services.AddScoped<IOrdersServices, OrderServices>();
            services.AddHttpContextAccessor();
            services.AddScoped<BusinessLogic.Services.Categorys.CategoryService>();
            services.AddScoped<BusinessLogic.Services.Products.ProductService>();


            var mailSettings = new MailSettings
            {
                Mail = EncryptData.ByteArrayToObject<string>(EncryptData.Decryption("NYi04pWx3H/MLeHzkOqQ78vAlxuJaJHo", "Supersic")),
                DisplayName = "ShopMMO",
                Password = EncryptData.ByteArrayToObject<string>(EncryptData.Decryption("urfqXaNYsrfCDm5ip3Gqr4MNdUzxIBve", "Supersic")),
                Host = "smtp.gmail.com",
                Port = 587
            }; 
            services.Configure<MailSettings>(options =>
            {
                options.Mail = mailSettings.Mail;
                options.DisplayName = mailSettings.DisplayName;
                options.Password = mailSettings.Password;
                options.Host = mailSettings.Host;
                options.Port = mailSettings.Port;
            });
            services.AddTransient<IEmailSender, SendMailService>();


            PayOS payOS = new PayOS(EncryptData.ByteArrayToObject<string>(EncryptData.Decryption("Lg/z97vcCzxz82hxxmXUPgfr8DkvUbFKwBTpsfmyTe8LvvjZONb4LA==", "Supersic")) ?? throw new Exception("Cannot find environment"),
                 EncryptData.ByteArrayToObject<string>(EncryptData.Decryption("Vh59AG+ltkNthRB245UzJDz3ecAEYVQY06N3Qjjs92qfJj8IQq2+pg==", "Supersic")) ?? throw new Exception("Cannot find environment"),
                EncryptData.ByteArrayToObject<string>(EncryptData.Decryption("LIEz2fnXXmwwZIcZLrNPuevmxV1LCvZvKTznQHNHrkpCphxT2ys1JtRXNU0owvMei0dgN899rvph291QHOw6YsgZQTNkEu/E", "Supersic")) ?? throw new Exception("Cannot find environment"));
            services.AddSingleton(payOS);
            services.AddAutoMapper(typeof(MappingProfile));


        }
    }
}
