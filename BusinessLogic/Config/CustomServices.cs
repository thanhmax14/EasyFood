using AutoMapper;
using BusinessLogic.Mapper;
using BusinessLogic.Services.Categorys;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Net.payOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessLogic.EmailServices.EmailService;

namespace BusinessLogic.Config
{
    public static class CustomServices
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();

















          
            var mailSettings = new MailSettings
            {
                Mail = "shopmmo.vn@gmail.com",
                DisplayName = "ShopMMO",
                Password = "znsb kkty fdfx nlxs",
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


            PayOS payOS = new PayOS("fa2021f3-d725-4587-a48f-8b55bccf7744" ?? throw new Exception("Cannot find environment"),
                  "143f45b5-d1d7-40e4-82e9-00ea8217ab33" ?? throw new Exception("Cannot find environment"),
                 "7861335ef9257ac91143d4de7b9f6ce64c864608defe1e31906510e95b345ee5" ?? throw new Exception("Cannot find environment"));
            services.AddSingleton(payOS);
            services.AddAutoMapper(typeof(MappingProfile));
        }
    }
}
