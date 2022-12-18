using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AdminLte.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class IsAuthenticated
    {
        private readonly RequestDelegate _next;

        public IsAuthenticated(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var user = httpContext.User;
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                string path = httpContext.Request.Path.Value;
                if (path.Contains("admin"))
                {
                    httpContext.Response.Redirect("/admin/");
                    return;
                     
                }
                else
                {
                    httpContext.Response.Redirect("/login/");
                    await _next(httpContext);
                    return;
                }
            }

            if (httpContext.User.Identity.IsAuthenticated && !httpContext.User.IsInRole("Admin"))
            {
                httpContext.Response.Redirect(httpContext.Request.Headers["Referer"].ToString());
                return;
            }
            else if (httpContext.User.Identity.IsAuthenticated && !httpContext.User.IsInRole("User"))
            {
                httpContext.Response.Redirect(httpContext.Request.Headers["Referer"].ToString());
                return;
            }

            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class IsAuthenticatedExtensions
    {
        public static IApplicationBuilder UseIsAuthenticated(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IsAuthenticated>();
        }
    }
}
