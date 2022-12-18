namespace AdminLte.Data.Entities
{
    public class Bank
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string? BankBranchName { get; set; }
        public string? BankBranchCity { get; set; }
        public string? BankBranchAddress { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? SwiftCode { get; set; }
        public bool IsDefault { get; set; }
        public bool Active { get; set; }
        public int CurrencyId { get; set; }
        //   public int AttachmentId { get; set; }
        public int CountryId { get; set; }
        public Currency Currency { get; set; }
        public Country Country { get; set; }
        public Attachment? Attachment { get; set; }

    }
}
