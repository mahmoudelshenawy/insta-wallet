using AdminLte.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminLte.Data.Configuration
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.Property(p => p.Balance).HasDefaultValue(0.00m);

            builder.HasOne(p => p.User).WithMany(u => u.Wallets).HasForeignKey(w => w.UserId);
        }
    }
}
