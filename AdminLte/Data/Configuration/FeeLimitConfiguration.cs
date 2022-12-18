using AdminLte.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminLte.Data.Configuration
{
    public class FeeLimitConfiguration : IEntityTypeConfiguration<FeeLimit>
    {
        public void Configure(EntityTypeBuilder<FeeLimit> builder)
        {
            builder.HasIndex(b => b.CurrencyId);

            builder.Property(b => b.FixedAmount).HasDefaultValue(0.00);
            builder.Property(b => b.PercentAmount).HasDefaultValue(0.00);
            builder.Property(b => b.MinLimit).HasDefaultValue(1.00);

            //builder.HasOne(b => b.Currency).WithMany(c => c.FeeLimits).HasForeignKey(w => w.CurrencyId);
            //builder.HasOne(b => b.PaymentMethod).WithMany(p => p.FeeLimits).HasForeignKey(w => w.PaymentMethodId);
            //builder.HasOne(b => b.TransactionTypes).WithMany(p => p.FeeLimits).HasForeignKey(w => w.TransactionTypeId);
        }
    }
}
