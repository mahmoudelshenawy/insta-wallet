using AdminLte.Models;
using AdminLte.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace AdminLte.Repositories
{
    public interface IAuthenticationRepository
    {
        Task<SignInResult> LoginUserAsync(LoginModel model);
        Task<SignInResult> LoginAdminInCookiesAsync(LoginModel userModel);
        Task<IdentityResult> CreateUserAsync(RegisterModel model);
         Task LogoutAsync();

        Task<ProfileModel> GetUserProfileData();
        Task<ProfileModel> UpdateUserProfileData(ProfileModel profileModel);

        Task GenerateEmailConfirmationTokenAsync(ApplicationUser user);
        Task GenerateResetPasswordConfirmationEmail(string email);
         Task<IdentityResult> ConfirmUserEmailAsync(string uid, string token);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordModel model, string uid, string token);
        Task<ApplicationUser> FindUserByEmailAsync(string email);
    }
}