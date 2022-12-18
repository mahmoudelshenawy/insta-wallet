using AdminLte.Areas.Repositories;
using AdminLte.Data;
using AdminLte.Data.Seeders;
using AdminLte.Middleware;
using AdminLte.Models;
using AdminLte.Providers;
using AdminLte.Repositories;
using AdminLte.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdminLte
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            var AdminAuthenticationScheme = "Admin";
            var UserAuthenticationScheme = "User";



            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
              {
                  options.LoginPath = _configuration["Application:LoginPath"];
              }).AddCookie(AdminAuthenticationScheme, options =>
              {
                  options.LoginPath = _configuration["Application:AdminLoginPath"];
              });


            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Cookie.Name = AdminAuthenticationScheme;
                options.LoginPath = _configuration["Application:LoginPath"];
                options.AccessDeniedPath = _configuration["Application:AccessDeniedPath"];
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", pl =>
                {
                    pl.AddAuthenticationSchemes(AdminAuthenticationScheme)
                    .RequireAuthenticatedUser();
                });

                options.AddPolicy("User", pl =>
               {
                   pl.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                   .RequireAuthenticatedUser();
               });

            });

            services.AddLocalization();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                options.SignIn.RequireConfirmedEmail = true;

            });

            services.AddControllersWithViews(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                //options.OutputFormatters.RemoveType<SystemTextJsonOutputFormatter>();
                //options.OutputFormatters.Add(new SystemTextJsonOutputFormatter(new JsonSerializerOptions(JsonSerializerDefaults.Web)
                //{
                //    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                //}));
            })
                .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });


            services.AddMvc(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(JsonStringLocalizerFactory));
    });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("ar-EG"),
                };
                options.DefaultRequestCulture = new RequestCulture(culture: supportedCultures[0], uiCulture: supportedCultures[0]);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthenticationUserRepository, AuthenticationUserRepository>();
            services.AddScoped<IDepositRepository, DepositRepository>();
            services.AddTransient<ISmsService, SmsService>();
            services.AddTransient<IStripeRepository, StripeRepository>();

            services.Configure<SMTPConfigModel>(_configuration.GetSection("SMTPConfig"));
            services.Configure<TwilioSettings>(_configuration.GetSection("Twilio"));

#if DEBUG
            //services.AddRazorPages().AddRazorRuntimeCompilation();
#endif

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();

            app.UseRouting();

            var supportedCultures = new[] { "en-US", "ar-EG" };
            var RequestLocalizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            app.UseRequestLocalization(RequestLocalizationOptions);

            app.UseAuthentication();
            app.UseAuthorization();

            // app.UseIsAuthenticated();

            // app.Map("/test",);
           // app.Map("/test", HandleRedirectFromAdminRoute);

            app.UseEndpoints(application =>
            {
                application.MapControllerRoute(
                 name: "admin",
                 pattern: "admin",
                 defaults: new { controller = "Home", action = "Index" });

                application.MapAreaControllerRoute(
                name: "/",
               areaName: "User",
               pattern: "{controller=Dashboard}/{action=Index}"
                );

                application.MapDefaultControllerRoute();
            });

        }

        static void HandleRedirectFromAdminRoute(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map Test 1" + context.User.HasClaim("role", "User").ToString() +
                    context.User.FindFirst("userId").Value);

            });
        }
        static void HandleRedirectFromAdmin2Route(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                context.Response.Redirect("/login");

                if (context.User.Identity.IsAuthenticated && !context.User.IsInRole("Admin"))
                {
                    await context.Response.WriteAsync("Map Test 2");
                    // context.Response.Redirect(context.Request.Headers["Referer"].ToString());
                }

                context.Response.Redirect("/login");
                // await context.Response.WriteAsync("Map Test 1");
            });
        }

    }
}

