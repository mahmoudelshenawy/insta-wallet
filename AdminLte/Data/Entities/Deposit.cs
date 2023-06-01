using AdminLte.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminLte.Data.Entities
{
    public class Deposit
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int CurrencyId { get; set; }
        public int PaymentMethodId { get; set; }
        public string? SentFrom { get; set; }
        public int? AttachmentId { get; set; }
        public string? Uuid { get; set; }
        public PaymentTypeEnum? PaymentType { get; set; }
        public int? PaymentTypeId { get; set; }   //which method wallet, payoneer, cards, bank 
        public int? BankId { get; set; }
        public decimal FixedFeeAmount { get; set; }
        public decimal? PercentFeeAmount { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public Currency Currency { get; set; }
        public Attachment? Attachment { get; set; }
        public Bank? Bank { get; set; }

        [NotMapped]
        public IServiceProvider ServiceProvider { get; set; }

        public async Task<Deposit> GetDepositAsync()
        {
            using (var scope = Startup._service.CreateScope())
            {
                using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var deposit = await context.Deposits.FirstOrDefaultAsync(x => x.Id == 2);
                return deposit;
            }
        }
    }


}
