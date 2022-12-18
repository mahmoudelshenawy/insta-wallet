using AdminLte.Models;
using AdminLte.Services;
using AdminLte.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Claims;

namespace AdminLte.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;

        public AuthenticationRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }
        public async Task<SignInResult> LoginUserAsync(LoginModel model)
        {
            string UserName;
            //check if user enter email or username
            if (new EmailAddressAttribute().IsValid(model.Email))
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                UserName = user?.UserName;
            }
            else
            {
                UserName = model.Email;
            }
            //var user =  _userManager.FindByEmailAsync(model.Email).Result.UserName;
            // string UserName = user != null ? user.UserName : model.Email;

            //  _signInManager.SignInWithClaimsAsync
            var result = await _signInManager.PasswordSignInAsync(UserName, model.Password, model.RememberMe, false);
            return result;
        }
        public async Task<SignInResult> LoginAdminInCookiesAsync(LoginModel userModel)
        {
            string UserName;
            var user = new ApplicationUser();
            if (new EmailAddressAttribute().IsValid(userModel.Email))
            {
                user = await _userManager.FindByEmailAsync(userModel.Email);
                UserName = user?.UserName;
            }
            else
            {
                UserName = userModel.Email;
                user = await _userManager.FindByNameAsync(userModel.Email);
            }

            var claims = new List<Claim>() {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, "Admin"),
                };

            var identity = new ClaimsIdentity(claims, "Admin");
            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext.SignInAsync("Admin",
                principal, new AuthenticationProperties()
                {
                    IsPersistent = userModel.RememberMe
                });
            var result = await _signInManager.PasswordSignInAsync(UserName, userModel.Password, userModel.RememberMe, false);
            return result;
        }
        public async Task<IdentityResult> CreateUserAsync(RegisterModel model)
        {
            ApplicationUser user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = new MailAddress(model.Email).User,
                Email = model.Email,
                NormalizedEmail = model.Email.ToUpper(),
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await GenerateEmailConfirmationTokenAsync(user);
            }

            return result;
        }
        public async Task<IdentityResult> ConfirmUserEmailAsync(string uid, string token)
        {
            var user = await _userManager.FindByIdAsync(uid);
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result;
        }
        public async Task GenerateEmailConfirmationTokenAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendConfirmationEmail(user, token);
            }
        }
        public async Task GenerateResetPasswordConfirmationEmail(string email)
        {
            var emailOptions = new UserEmailOptions
            {
                ToEmails = new List<string> { email },
            };
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await SendResetPasswordConfirmationEmail(user, token);
            }
        }
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            await _httpContextAccessor.HttpContext.SignOutAsync("Admin");
            await _httpContextAccessor.HttpContext.ChallengeAsync("Admin");
        }
        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model, string uid, string token)
        {
            var user = await _userManager.FindByIdAsync(uid);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                return result;
            }
            else
            {
                throw new Exception();
            }
        }
        public async Task<ApplicationUser> FindUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }
        public async Task<ProfileModel> GetUserProfileData()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var User = await _userManager.GetUserAsync(user);

            ProfileModel ProfileModel = new ProfileModel()
            {
                UserName = User.UserName,
                FirstName = User.FirstName,
                LastName = User.LastName,
                PhoneNumber = User.PhoneNumber,
                ProfilePath = User.Profile
            };
            return ProfileModel;

            //return ProfileModel;
            //var userName = await _userManager.GetUserNameAsync(user);
            //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

        }

        public async Task<ProfileModel> UpdateUserProfileData(ProfileModel profileModel)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var User = await _userManager.GetUserAsync(user);
            if (profileModel.FirstName != User.FirstName)
            {
                User.FirstName = profileModel.FirstName;
                await _userManager.UpdateAsync(User);
            }
            if (profileModel.LastName != User.LastName)
            {
                User.LastName = profileModel.LastName;
                await _userManager.UpdateAsync(User);
            }
            if (profileModel.PhoneNumber != User.PhoneNumber)
            {
                User.PhoneNumber = profileModel.PhoneNumber;
                await _userManager.UpdateAsync(User);
            }
            if (profileModel.ProfilePath != null)
            {
                User.Profile = profileModel.ProfilePath;
                await _userManager.UpdateAsync(User);
            }
            return profileModel;

        }

        private async Task SendConfirmationEmail(ApplicationUser user, string token)
        {
            var appDomain = _configuration.GetSection("Application:AppDomain").Value;
            var EmailConfirmation = _configuration.GetSection("Application:EmailConfirmation").Value;
            UserEmailOptions userEmailOptions = new UserEmailOptions
            {
                ToEmails = new List<string> { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string> ("{{UserName}}" , user.FirstName ),
                    new KeyValuePair<string, string> ("{{Link}}" , String.Format(appDomain + EmailConfirmation , user.Id , token ))
                }
            };
            await _emailService.SendConfirmationEmail(userEmailOptions);
        }
        private async Task SendResetPasswordConfirmationEmail(ApplicationUser user, string token)
        {
            var appDomain = _configuration.GetSection("Application:AppDomain").Value;
            var EmailConfirmation = _configuration.GetSection("Application:ForgotPassword").Value;
            UserEmailOptions userEmailOptions = new UserEmailOptions
            {
                ToEmails = new List<string> { user.Email },
                PlaceHolders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string> ("{{UserName}}" , user.FirstName ),
                    new KeyValuePair<string, string> ("{{Link}}" , String.Format(appDomain + EmailConfirmation , user.Id , token ))
                }
            };
            await _emailService.SendResetPasswordEmail(userEmailOptions);
        }
    }
}
