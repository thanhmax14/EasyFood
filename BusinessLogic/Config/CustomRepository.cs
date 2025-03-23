
﻿using BusinessLogic.Services.BalanceChanges;
using BusinessLogic.Services.Categorys;
using BusinessLogic.Services.ProductImages;
using BusinessLogic.Services.ProductVariants;
using BusinessLogic.Services.ProductVariantVariants;
using BusinessLogic.Services.Reviews;
using Microsoft.Extensions.DependencyInjection;
using Repository.BalanceChange;
using Repository.Carts;
using Repository.ProductImage;
using Repository.Products;
using Repository.ProductVariants;
using Repository.Reviews;
using Repository.StoreDetails;
﻿using Microsoft.Extensions.DependencyInjection;
using Repository.Categorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Wishlists;
using Repository.OrdersRepository;
using Repository.OrdeDetails;

namespace BusinessLogic.Config
{
    public static class CustomRepository
    {
        public static void ConfigureRepository(this IServiceCollection services)
        {
            services.AddScoped<IStoreDetailsRepository, StoreDetailsRepository>();
            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<IBalanceChangeRepository, BalanceChangeRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IWishlistRepository, WishlistRepository>();
            services.AddScoped<IOrdersRepository, OrdersRepository>();
            services.AddScoped<Repository.StoreDetails.StoreDetailsRepository>();
            services.AddScoped<Repository.Categorys.CategoryRepository>();
            services.AddScoped<Repository.Products.ProductsRepository>();
            services.AddScoped<Repository.ProductImage.ProductImageRepository>();
            services.AddScoped<Repository.ProductVariants.ProductVariantRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();


        }



        }
}
