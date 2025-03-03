
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
        }



        }
}
