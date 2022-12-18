using AdminLte.Models;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace AdminLte.Services
{
    public class SmsService : ISmsService
    {
        private readonly TwilioSettings _twilio;

        public SmsService(IOptions<TwilioSettings> twillio)
        {
            _twilio = twillio.Value;
        }

        public MessageResource Send(string mobileNumber, string body)
        {
            TwilioClient.Init(_twilio.AccountSID, _twilio.AuthToken);

            var result = MessageResource.Create(
                  body: body,
                  from: new Twilio.Types.PhoneNumber(_twilio.TwilioPhoneNumber),
                  to: mobileNumber
              );

            return result;
        }

    }
}
