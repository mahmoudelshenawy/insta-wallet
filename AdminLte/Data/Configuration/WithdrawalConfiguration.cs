using AdminLte.Data.Entities;
using AdminLte.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminLte.Data.Configuration
{
    public class WithdrawalConfiguration : IEntityTypeConfiguration<Withdrawal>
    {
        public void Configure(EntityTypeBuilder<Withdrawal> builder)
        {
            builder.Property(b => b.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
            builder.Property(b => b.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
            builder.Property(b => b.FixedFeeAmount).HasDefaultValue(0.00m);
            builder.Property(b => b.Status).HasConversion(n => n.ToString(), t => Enum.Parse<PaymentStatus>(t));
            builder.Property(b => b.PaymentType).HasConversion(n => n.ToString(), t => Enum.Parse<WithdrawalPaymentMethodsEnum>(t));

            builder.HasOne(b => b.PaymentMethod).WithMany().HasForeignKey(b => b.PaymentMethodId);
            builder.HasOne(b => b.Currency).WithMany().HasForeignKey(b => b.CurrencyId);
            builder.HasOne(b => b.WithdrawalDetail).WithOne();
            builder.HasOne(b => b.PayoutSetting).WithMany().HasForeignKey(d => d.PayoutSettingId);
        }
    }
}
