using AdminLte.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminLte.Data.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string EndUserId { get; set; }
        public int CurrencyId { get; set; }
        public int PaymentMethodId { get; set; }
        public string? SentFrom { get; set; }
        public int AttachmentId { get; set; }
        public int ConfirmAttachmentId { get; set; }
        public string? Uuid { get; set; }
        public PaymentTypeEnum? PaymentType { get; set; } 
        public int PaymentTypeId { get; set; }   //which method wallet, payoneer, cards, bank 
        public int? TransactionTypeId { get; set; }
        public int? BankId { get; set; }

        public int TransactionReferenceId { get; set; } //which deposit,withdrawal,transfer,exchange
        public string? RefundReference { get; set; }

        public UserRegisterType UserType { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal FixedFeeAmount { get; set; }
        public decimal? PercentFeeAmount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }

        public string? Note { get; set; }
        public string? Extra { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("EndUserId")]
        public virtual ApplicationUser EndUser { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Attachment Attachment { get; set; }
        [ForeignKey("ConfirmAttachmentId")]
        public virtual Attachment ConfirmFile { get; set; }
        public virtual Bank? Bank { get; set; }

        [ForeignKey("TransactionTypeId")]
        public virtual TransactionTypes TransactionType { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,
        Success,
        Refund,
        Blocked
    }

    public enum UserRegisterType
    {
        Registered,
        Unregistered
    }
    public enum DepositStatus
    {
        Pending,
        Success,
        Refund,
        Blocked
    }

    public enum PaymentTypeEnum
    {
        Bank,
        PayeerWallet,
        Strip,
        Paypal,
        Paymob,
        VodafoneCash,
        PerfectMoney,
        Payooner,
        Bitcoin,
        Usdt
    }
}
