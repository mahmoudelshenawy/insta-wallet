using AdminLte.Areas.Repositories;
using AdminLte.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Twilio.Rest.Api.V2010.Account;

namespace AdminLte.Areas.User.Controllers
{
    [Authorize(Policy = "User")]
    [Area("User")]
    [Route("/")]
    public class DashboardController : Controller
    {
        private readonly ISmsService _smsService;
        private readonly IStripeRepository _stripeRepository;

        public DashboardController(ISmsService smsService, IStripeRepository stripeRepository)
        {
            _smsService = smsService;
            _stripeRepository = stripeRepository;
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
        public IActionResult TestStripe()
        {
            var customers = _stripeRepository.DoStuffInStripe();
            return Ok(customers);
        }
    }
}
