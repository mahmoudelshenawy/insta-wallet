using AdminLte.Areas.Repositories;
using AdminLte.Data;
using AdminLte.Models;
using AdminLte.Providers;
using AdminLte.Repositories;
using AdminLte.Rules;
using AdminLte.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;

namespace AdminLte.Configuration
{
    public class ProjectSetupInstaller : IServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration _configuration)
        {
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthenticationUserRepository, AuthenticationUserRepository>();
            services.AddScoped<IDepositRepository, DepositRepository>();
            services.AddScoped<IWithdrawalRepository, WithdrawalRepository>();
            services.AddTransient<ISmsService, SmsService>();
            services.AddTransient<IStripeRepository, StripeRepository>();
            services.AddTransient<IApiAuthenticationRepository, ApiAuthenticationRepository>();
            services.AddSingleton<DapperDbContext>();

        }
    }
}
