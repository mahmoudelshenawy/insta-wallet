using AdminLte.ViewModels;

namespace AdminLte.Services
{
    public interface IEmailService
    {
        Task SendConfirmationEmail(UserEmailOptions userEmailOptions);
        Task SendResetPasswordEmail(UserEmailOptions userEmailOptions);
    }
}