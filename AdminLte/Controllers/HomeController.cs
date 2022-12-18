using AdminLte.Data.Enums;
using AdminLte.Models;
using AdminLte.Repositories;
using AdminLte.Services;
using AdminLte.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Security.Claims;

namespace AdminLte.Controllers
{
    [Authorize(Policy = "Admin")]
    [Route("admin")]
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;
        private readonly IAuthenticationRepository _AuthenticationRepository;
        private readonly IUploadService _UploadService;

        public HomeController(IStringLocalizer<HomeController> localizer,
            IAuthenticationRepository authenticationRepository, IUploadService uploadService)
        {
            _localizer = localizer;
            _AuthenticationRepository = authenticationRepository;
            _UploadService = uploadService;

        }

         [HttpGet("home")]
        //[Authorize(Roles = "Admin")]
        public IActionResult Index()
        {

            // return User.FindFirst(u => u.Type == ClaimTypes.Name).Value;
            var Prop = "Name";
            var Currency = new RoleModel
            {
                Name = "test"
            };
            var prop = Currency.GetType().GetProperty(Prop);
            var test = prop.GetValue(Currency);
            var res = prop.Equals("test");
            ViewBag.welcome = _localizer["welcome"];
            ViewBag.item = "hello";
            return View();
        }

        [AllowAnonymous]
        [HttpPost("SetLanguage")]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTime.UtcNow.AddYears(1) }

                );
            return LocalRedirect(returnUrl);
        }

        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var profileData = await _AuthenticationRepository.GetUserProfileData();
            ViewBag.profileData = profileData;

            return View(profileData);
        }
        [HttpPost("profile")]
        public async Task<IActionResult> Profile(ProfileModel profileModel)
        {
            if (ModelState.IsValid)
            {
                if (profileModel.ProfileImage != null)
                {
                    profileModel.ProfilePath = await _UploadService.UploadImage("uploads/profiles/", profileModel.ProfileImage);
                }
                var profileResult = await _AuthenticationRepository.UpdateUserProfileData(profileModel);
                if (profileResult != null)
                {
                    ViewBag.IsSuccess = true;
                }
                return View(profileResult);
            }
            var profileData = await _AuthenticationRepository.GetUserProfileData();
            ViewBag.profileData = profileData;

            return View(profileData);
        }

    }
}
