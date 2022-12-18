using AdminLte.Models;
using AdminLte.ViewModels;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AdminLte.Services
{
    public class EmailService : IEmailService
    {
        private const string templatePath = @"EmailTemplate/{0}.html";
        private readonly SMTPConfigModel _smtpConfig;

        public EmailService(IOptions<SMTPConfigModel> smtpConfig)
        {
            _smtpConfig = smtpConfig.Value;
        }

        private async Task SendEmail(UserEmailOptions userEmail)
        {
            MailMessage mail = new MailMessage()
            {
                Subject = userEmail.Subject,
                Body = userEmail.Body,
                From = new MailAddress(_smtpConfig.SenderAddress, _smtpConfig.SenderDisplayName),
                IsBodyHtml = _smtpConfig.IsBodyHTML
            };

            foreach (var toEmail in userEmail.ToEmails)
            {
                mail.To.Add(toEmail);
            }

            NetworkCredential network = new NetworkCredential(_smtpConfig.UserName, _smtpConfig.Password);

            SmtpClient smtpClient = new SmtpClient()
            {
                Host = _smtpConfig.Host,
                Port = _smtpConfig.Port,
                EnableSsl = _smtpConfig.EnableSSL,
                UseDefaultCredentials = _smtpConfig.UseDefaultCredentials,
                Credentials = network
            };
            mail.BodyEncoding = Encoding.Default;

            await smtpClient.SendMailAsync(mail);

        }

        private string GetEmailBody(string templateName)
        {
            var body = File.ReadAllText(string.Format(templatePath, templateName));
            return body;
        }

        private string UpdatePlaceHolders(string text, List<KeyValuePair<string, string>> keyValuePairs)
        {
            if (!string.IsNullOrEmpty(text) && keyValuePairs != null)
            {
                foreach (var keyValue in keyValuePairs)
                {
                    if (text.Contains(keyValue.Key))
                    {
                        text = text.Replace(keyValue.Key, keyValue.Value);
                    }
                }
            }
            return text;
        }

        public async Task SendConfirmationEmail(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = UpdatePlaceHolders("Confirm Your Email", userEmailOptions.PlaceHolders);
            userEmailOptions.Body = UpdatePlaceHolders(GetEmailBody("ConfirmEmail"), userEmailOptions.PlaceHolders);
            await SendEmail(userEmailOptions);
        } 
        public async Task SendResetPasswordEmail(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = UpdatePlaceHolders("Reset Your Password", userEmailOptions.PlaceHolders);
            userEmailOptions.Body = UpdatePlaceHolders(GetEmailBody("ForgotPassword"), userEmailOptions.PlaceHolders);
            await SendEmail(userEmailOptions);
        }
    }
}
