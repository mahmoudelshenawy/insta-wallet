using AdminLte.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace AdminLte.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Profile { get; set; }
        public string? FormattedPhone { get; set; }
        public string Type { get; set; }
        public bool AddressVerified { get; set; }
        public bool IdentityVerified { get; set; }
        public string? Status { get; set; }
        public string? CarrierCode { get; set; }
        public string? DefaultCountry { get; set; }
        public List<Wallet> Wallets { get; set; }

        public List<RefreshToken>? RefreshTokens { get; set; }
    }
}
