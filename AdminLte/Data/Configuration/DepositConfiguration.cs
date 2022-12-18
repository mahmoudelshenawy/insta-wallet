using AdminLte.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminLte.Data.Configuration
{
    public class DepositConfiguration : IEntityTypeConfiguration<Deposit>
    {
        public void Configure(EntityTypeBuilder<Deposit> builder)
        {
            builder.Property(b => b.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
            builder.Property(b => b.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
           // builder.Property(b => b.Uuid).HasDefaultValue(Guid.NewGuid().ToString("D"));
            builder.Property(b => b.FixedFeeAmount).HasDefaultValue(0.00m);

            builder.Property(b => b.Status).HasConversion(n => n.ToString(), t => Enum.Parse<PaymentStatus>(t));
            builder.Property(b => b.PaymentType).HasConversion(n => n.ToString(), t => Enum.Parse<PaymentTypeEnum>(t));

            builder.HasOne(b => b.PaymentMethod).WithMany(p => p.Deposits).HasForeignKey(b => b.PaymentMethodId);
            builder.HasOne(b => b.Currency).WithMany(p => p.Deposits).HasForeignKey(b => b.CurrencyId);
            
        }
    }
}
