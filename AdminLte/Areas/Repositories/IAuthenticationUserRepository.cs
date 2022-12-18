using AdminLte.Areas.User.Models;
using AdminLte.Models;
using Microsoft.AspNetCore.Identity;

namespace AdminLte.Areas.Repositories
{
    public interface IAuthenticationUserRepository
    {
        Task<IdentityResult> CreateUserAsync(RegisterUserModel userModel);

         Task<SignInResult> LoginUserAsync(LoginUserModel userModel);
        Task<IdentityResult> ConfirmUserEmailAsync(string uid, string token);

        Task<bool> SendResetPasswordTokenAsync(string email);

         Task LogoutAsync();

        Task<bool> LoginUserInCookiesAsync(LoginUserModel userModel);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel passwordModel);
        Task<bool> EmailIsTaken(string email);

        Task<bool> PhoneIsTaken(string phone);
    }
}