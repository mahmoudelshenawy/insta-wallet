using AdminLte.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminLte.Data.Configuration
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.Property(b => b.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
            builder.Property(b => b.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();
            builder.Property(b => b.FixedFeeAmount).HasDefaultValue(0.00m);
            builder.Property(b => b.Subtotal).HasDefaultValue(0.00m);
            builder.Property(b => b.Total).HasDefaultValue(0.00m);

            builder.Property(b => b.Status).HasConversion(n => n.ToString(), t => Enum.Parse<PaymentStatus>(t));
            builder.Property(b => b.PaymentType).HasConversion(n => n.ToString(), t => Enum.Parse<PaymentTypeEnum>(t));
            builder.Property(b => b.UserType).HasConversion(n => n.ToString(), t => Enum.Parse<UserRegisterType>(t));

            builder.HasOne(b => b.PaymentMethod).WithMany(p => p.Transactions).HasForeignKey(b => b.PaymentMethodId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(b => b.Currency).WithMany(p => p.Transactions).HasForeignKey(b => b.CurrencyId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(b => b.Attachment).WithOne().OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(b => b.ConfirmFile).WithOne().OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(b => b.User).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(b => b.Bank).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(b => b.TransactionType).WithMany().OnDelete(DeleteBehavior.NoAction);
        }
    }
}
