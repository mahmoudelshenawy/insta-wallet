using AdminLte.Data.Entities;

namespace AdminLte.Areas.User.Models
{
    public class PayoutSettingViewModel
    {
        public int? PayoutSettingId { get; set; }
        public bool SavePayoutSetting { get; set; }
        public bool NewPayoutSetting { get; set; }

        public BankSetting? BankSetting { get; set; }
        public CashSetting? CashSetting { get; set; }
        public WalletSetting? VodafoneCashSetting { get; set; }
        public WalletSetting? PayeerWalletSetting { get; set; }
        public WalletSetting? UsdtSetting { get; set; }
        public WalletSetting? PerfectMoneySetting { get; set; }
        public WalletSetting? OrangeMoneySetting { get; set; }
        public CryptoWalletSetting? CryptoWalletSetting { get; set; }
        public PaypalSetting? PaypalSetting { get; set; }
        public PayoneerSetting? PayoneerSetting { get; set; }
    }

    public class BankSetting
    {
        public string BankName { get; set; }
        public string? BankBranchName { get; set; }
        public string? BankBranchCity { get; set; }
        public string? BankBranchAddress { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? SwiftCode { get; set; }
        public int? CountryId { get; set; }
        public Country Country { get; set; }
    }

    public class CashSetting
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string PreferredMethod { get; set; }
        public int? CountryId { get; set; }
        public Country Country { get; set; }
    }
    public class WalletSetting
    {
        public string? MethodName { get; set; }
        public string? WalletName { get; set; }
        public string WalletNumber { get; set; }
        public string Description { get; set; }
    }

    public class CryptoWalletSetting
    {
        public string? WalletName { get; set; }
        public string WalletNumber { get; set; }
        public string WalletAddress { get; set; }
        public string Description { get; set; }
    }
    public class PayoneerSetting
    {
        public string PayeeEmail { get; set; }
    }
    public class PaypalSetting
    {
        public string Email { get; set; }
        
    }
}
