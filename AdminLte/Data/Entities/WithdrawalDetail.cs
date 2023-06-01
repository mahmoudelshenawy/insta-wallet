namespace AdminLte.Data.Entities
{
    public class WithdrawalDetail
    {
        public int Id { get; set; }
        public int WithdrawalId { get; set; }
        public string? BankSetting { get; set; }
        public string? CashSetting { get; set; }
        public string? WalletSetting { get; set; }
        public string? PaypalSetting { get; set; }
        public string? PayoneerSetting { get; set; }
    }
}
