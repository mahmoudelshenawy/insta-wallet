using AdminLte.Areas.Repositories;
using AdminLte.Areas.User.Models;
using AdminLte.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminLte.Areas.User.Controllers
{
    [Area("User")]
    [AllowAnonymous]
    [Route("")]
    public class AuthenticationController : Controller
    {
        public readonly IAuthenticationUserRepository _AuthRepo;

        public AuthenticationController(IAuthenticationUserRepository authRepo)
        {
            _AuthRepo = authRepo;
        }


        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterUserModel userModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _AuthRepo.CreateUserAsync(userModel);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                return RedirectToAction("ConfirmEmail", "Authentication", new { email = userModel.Email });
            }
            return View("Register");
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string uid, string token, string email)
        {
            var confirmEmail = new EmailConfirmModel
            {
                Email = email,
            };

            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty("token"))
            {
                token = token.Replace(' ', '+');
                var result = await _AuthRepo.ConfirmUserEmailAsync(uid, token);
                if (result.Succeeded)
                {
                    confirmEmail.EmailVerified = true;
                }
            }
            return View(confirmEmail);
        }
        [AllowAnonymous]
        [HttpGet("/login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("User"))
            {
                if (!string.IsNullOrEmpty(Request.Headers["Referer"].ToString()))
                {
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                else
                {
                    return RedirectToAction("Index", "Dashboard");
                }

            }

            return View();
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginUserModel userModel)
        {
            //login using email or username

            if (ModelState.IsValid)
            {
                //var result = await _AuthRepo.LoginUserAsync(userModel);

                //if (result.Succeeded)
                //{
                //    return RedirectToAction("Index", "Dashboard");
                //}

                var result = await _AuthRepo.LoginUserInCookiesAsync(userModel);

                if (result == true)
                {
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            return View("Login");
        }

        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            var forgotPassword = new ForgotPasswordModel();
            return View(forgotPassword);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordPost(ForgotPasswordModel passwordModel)
        {
            var result = await _AuthRepo.SendResetPasswordTokenAsync(passwordModel.Email);

            if (result == true)
            {
                passwordModel.EmailSent = true;
            }

            return View("ForgotPassword", passwordModel);
        }
        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string uid, string token)
        {
            var resetPassword = new ResetPasswordModel
            {
                userId = uid,
                Token = token
            };
            return View(resetPassword);
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel passwordModel)
        {

            if (!string.IsNullOrEmpty(passwordModel.userId) && !string.IsNullOrEmpty(passwordModel.Token))
            {
                passwordModel.Token = passwordModel.Token.Replace(' ', '+');
                var result = await _AuthRepo.ResetPasswordAsync(passwordModel);

                if (result.Succeeded)
                {
                    passwordModel.IsReset = true;
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(passwordModel);
        }

        [HttpGet("check-email-exists/{email}")]
        public async Task<IActionResult> EmailExists(string email)
        {

            var result = await _AuthRepo.EmailIsTaken(email);

            var data = new
            {
                exists = result
            };
            return Ok(data);
        }
        [HttpGet("check-phone-exists/{phone}")]
        public async Task<IActionResult> PhoneExists(string phone)
        {

            var result = await _AuthRepo.PhoneIsTaken(phone);

            var data = new
            {
                exists = result
            };
            return Ok(data);
        }
        [HttpGet("logout")]
        public async Task<IActionResult> LgoutOut()
        {
            await _AuthRepo.LogoutAsync();
            return Redirect("/login");
        }
    }
}
