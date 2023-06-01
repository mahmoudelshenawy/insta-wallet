using AdminLte.Areas.Repositories;
using AdminLte.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AdminLte.Areas.User.Controllers
{
    [Authorize(Policy = "User")]
    [Area("User")]
    [Route("/")]
    public class DashboardController : Controller
    {
        private readonly IStripeRepository _stripeRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        public DashboardController(IStripeRepository stripeRepository, IHttpClientFactory httpClientFactory)
        {
            _stripeRepository = stripeRepository;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("dashboard")]
        [HttpGet("/")]
        public object Index()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTime.UtcNow.AddYears(1) }

                );
            return LocalRedirect(returnUrl);
        }

        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult TestStripeTwo()
        {
            var customers = _stripeRepository.DoStuffInStripe();
            return Ok(customers);
        }
        [HttpGet("test/api")]
        [AllowAnonymous]
        public async Task<IActionResult> TestHttpClient()
        {
           using(var client = new HttpClient())
            {
                var uri = new Uri("https://jsonplaceholder.typicode.com/posts");
                //var result = await client.GetAsync(uri);
                //var json = await result.Content.ReadAsStringAsync();

                var post = new Post
                {
                    Title = "Post title",
                    Body = "post body",
                    UserId = 22
                };
               var postJson = JsonSerializer.Serialize(post);
                var payload = new StringContent(postJson, Encoding.UTF8, "application/json");
                var result = await client.PostAsync(uri, payload);
                var json = result.Content.ReadAsStringAsync();
                return Ok(json);
            }

            return Ok();
        }
    }

    public class Post
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int UserId { get; set; }
    }
}
