using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AdminLte.Configuration
{
    public class AuthenticationAndAuthorizationServiceInstaller
    {
        public void Install(IServiceCollection services, IConfiguration configuration)
        {
            var AdminAuthenticationScheme = "Admin";
            var UserAuthenticationScheme = "User";
            var bearerAuthenticationScheme = JwtBearerDefaults.AuthenticationScheme;


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
              {
                  options.LoginPath = configuration["Application:LoginPath"];
              }).AddCookie(AdminAuthenticationScheme, options =>
              {
                  options.LoginPath = configuration["Application:AdminLoginPath"];

              }).AddJwtBearer(o =>
              {
                  o.RequireHttpsMetadata = false;
                  o.SaveToken = false;
                  o.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = true,
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidIssuer = configuration["JWT:Issuer"],
                      ValidAudience = configuration["JWT:Audience"],
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
                      ClockSkew = TimeSpan.Zero
                  };
              });


            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Cookie.Name = AdminAuthenticationScheme;
                options.LoginPath = configuration["Application:LoginPath"];
                options.AccessDeniedPath = configuration["Application:AccessDeniedPath"];
            });


            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", pl =>
                {
                    pl.AddAuthenticationSchemes(AdminAuthenticationScheme)
                    .RequireAuthenticatedUser().RequireClaim("Admin");
                });

                options.AddPolicy("User", pl =>
                {
                    pl.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser();
                });

            });
        }
    }
}
