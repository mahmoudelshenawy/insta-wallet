using AdminLte.Areas.User.Models;
using AdminLte.Data;
using AdminLte.Data.Entities;
using AdminLte.Models;
using AdminLte.Services;
using AdminLte.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Claims;

namespace AdminLte.Areas.Repositories
{
    public class AuthenticationUserRepository : IAuthenticationUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        public AuthenticationUserRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor, IEmailService emailService, ApplicationDbContext context, ISmsService smsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _context = context;
            _smsService = smsService;
        }


        public async Task<IdentityResult> CreateUserAsync(RegisterUserModel userModel)
        {
            var emailExist = await _userManager.FindByEmailAsync(userModel.Email);

            if (emailExist != null)
            {
                var error = new IdentityError
                {
                    Code = "EmailExists",
                    Description = "Email is already exists"
                };
                return IdentityResult.Failed(error);
            }

            var phoneExists = await _userManager.Users.Where(u => u.PhoneNumber == userModel.Phone).FirstOrDefaultAsync();
            if (phoneExists != null)
            {
                var error = new IdentityError
                {
                    Code = "PhoneExists",
                    Description = "Phone is already exists"
                };
                return IdentityResult.Failed(error);
            }

            var newUser = new ApplicationUser
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                PhoneNumber = userModel.Phone,
                FormattedPhone = userModel.FormattedPhone,
                Type = userModel.Type,
                CarrierCode = userModel.CarrierCode,
                DefaultCountry = userModel.DefaultCountry,
                Email = userModel.Email,
                UserName = new MailAddress(userModel.Email).User,
                NormalizedUserName = new MailAddress(userModel.Email).User.ToUpper(),
                NormalizedEmail = userModel.Email.ToUpper(),
                Status = "Active"
            };

            var result = await _userManager.CreateAsync(newUser, userModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "User");

                //send confirmation email
                await GenerateConfirmationTokenAsync(newUser);

                //send welcome sms message
                SendWelcomeMessageToUser(newUser.FormattedPhone);

                //create default wallet
                await CreateDefaultWalletForNewRegisteredUser(newUser);

                //create Qrcode for user 

                //check email existence in payments and transactions
            }
            return result;
        }
        public async Task GenerateConfirmationTokenAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            if (!string.IsNullOrEmpty(token))
            {
                await SendConfirmationEmail(user, token);
            }
        }

        public async Task<SignInResult> LoginUserAsync(LoginUserModel userModel)
        {
            string UserName;
            if (new EmailAddressAttribute().IsValid(userModel.Email))
            {
                var user = await _userManager.FindByEmailAsync(userModel.Email);
                UserName = user?.UserName;
            }
            else
            {
                UserName = userModel.Email;
            }
            return await _signInManager.PasswordSignInAsync(UserName, userModel.Password, userModel.RememberMe, false);
        }

        public async Task<bool> LoginUserInCookiesAsync(LoginUserModel userModel)
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
                        new Claim(ClaimTypes.Role, "User"),
                        new Claim("me", "mah"),
                };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                principal, new AuthenticationProperties()
                {
                    IsPersistent = userModel.RememberMe
                });
            return true;
        }
        public async Task<IdentityResult> ConfirmUserEmailAsync(string uid, string token)
        {
            return await _userManager.ConfirmEmailAsync(await _userManager.FindByIdAsync(uid), token);
        }
        public async Task<bool> EmailIsTaken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user != null ? true : false;
        }
        public async Task<bool> PhoneIsTaken(string phone)
        {
            var user = await _userManager.Users.Where(u => u.PhoneNumber == phone).FirstOrDefaultAsync();

            return user != null ? true : false;
        }

        public async Task<bool> SendResetPasswordTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            if (!string.IsNullOrEmpty(token))
            {
                await SendResetPasswordTokenEmail(user, token);
                return true;
            }
            return false;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel passwordModel)
        {
            return await _userManager.ResetPasswordAsync(await _userManager.FindByIdAsync(passwordModel.userId),
               passwordModel.Token, passwordModel.NewPassword);
        }
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        ////////////////////////Private methods/////////////////////////////////////////////////////////////////////////////////////
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
        private async Task SendResetPasswordTokenEmail(ApplicationUser user, string token)
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


        private async Task CreateDefaultWalletForNewRegisteredUser(ApplicationUser user)
        {
            var defaultCurrencyId = await _context.Settings.Where(s => s.Name == "DefaultCurrency").FirstOrDefaultAsync();
            if (!string.IsNullOrEmpty(defaultCurrencyId.Value))
            {
                var currency = await _context.Currencies
                    .Where(c => c.Id == Convert.ToInt32(defaultCurrencyId.Value))
                    .FirstOrDefaultAsync();

                var wallet = new Wallet
                {
                    UserId = user.Id,
                    CurrencyId = currency.Id,
                    IsDefault = true
                };

                _context.Add(wallet);
                _context.SaveChangesAsync();
            }
        }

        private bool SendWelcomeMessageToUser(string phoneNumber)
        {
            string body = "Welcome to our website";
            var result = _smsService.Send(phoneNumber, body);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return false;

            return true;
        }
    }
}
