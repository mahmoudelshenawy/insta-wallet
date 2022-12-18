using AdminLte.Models;
using AdminLte.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdminLte.Controllers
{
    [Route("admin")]
    //[Authorize(Policy = "Admin")]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationRepository _authenticationRepository;

        public AuthenticationController(IAuthenticationRepository iauthenticationRepository)
        {
            _authenticationRepository = iauthenticationRepository;
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authenticationRepository.CreateUserAsync(model);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
                return RedirectToAction("ConfirmEmail", new { email = model.Email });
            }
            return View();
        }

        [AllowAnonymous, HttpGet("")]
        public IActionResult Login()
        {
            //if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            //{
            //    if (!string.IsNullOrEmpty(Request.Headers["Referer"].ToString())){
            //        return Redirect(Request.Headers["Referer"].ToString());
            //    }
            //    else
            //    {
            //        return RedirectToAction("Index", "Home");
            //    }

            //}
            if(User.FindFirstValue(ClaimTypes.Role) == "Admin")
            {

            }
            if(User.HasClaim(ClaimTypes.Role , "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _authenticationRepository.LoginAdminInCookiesAsync(model);

                //if (result == true)
                //{
                //    if (!string.IsNullOrEmpty(ReturnUrl))
                //    {
                //        return LocalRedirect(ReturnUrl);
                //    }

                //    return RedirectToAction("Index", "Home");
                //}

               // var result = await _authenticationRepository.LoginUserAsync(model);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl))
                    {
                        return LocalRedirect(ReturnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsNotAllowed == true)
                {
                    ModelState.AddModelError("", "Not Allowed");
                }
                else
                {
                    ModelState.AddModelError("", "invalid credentials");
                }
            }
            return View();
        }

        [HttpGet("logout")]
        public async Task<IActionResult> LgoutOut()
        {
            await _authenticationRepository.LogoutAsync();
            
            return Redirect("/admin");
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string uid, string token, string email)
        {
            var EmailConfirm = new EmailConfirmModel
            {
                Email = email
            };
            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
            {
                token = token.Replace(' ', '+');
                var result = await _authenticationRepository.ConfirmUserEmailAsync(uid, token);

                if (result.Succeeded)
                {
                    EmailConfirm.EmailVerified = true;

                }
            }
            return View(EmailConfirm);

        }
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(EmailConfirmModel confirmModel)
        {
            var user = await _authenticationRepository.FindUserByEmailAsync(confirmModel.Email);
            if (user == null)
                return NotFound();

            if (user.EmailConfirmed == true)
            {
                confirmModel.EmailVerified = true;
                return View(confirmModel);
            }

            await _authenticationRepository.GenerateEmailConfirmationTokenAsync(user);

            confirmModel.EmailSent = true;
            ModelState.Clear();


            return View(confirmModel);

        }
        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {

            return View();

        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                await _authenticationRepository.GenerateResetPasswordConfirmationEmail(model.Email);
                model.EmailSent = true;
                ModelState.Clear();
            }
            return View(model);

        }
        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string uid, string token)
        {
            var ResetPassword = new ResetPasswordModel()
            {
                userId = uid,
                Token = token
            };
            if (!string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(token))
            {
                token = token.Replace(' ', '+');
            }

            return View(ResetPassword);

        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel passwordModel)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(passwordModel.userId) && !string.IsNullOrEmpty(passwordModel.Token))
                {
                    var token = passwordModel.Token.Replace(' ', '+');
                    var result = await _authenticationRepository.ResetPasswordAsync(passwordModel, passwordModel.userId, token);

                    if (result.Succeeded)
                    {
                        passwordModel.IsReset = true;

                    }
                }
            }
            return View(passwordModel);

        }

    }


}
