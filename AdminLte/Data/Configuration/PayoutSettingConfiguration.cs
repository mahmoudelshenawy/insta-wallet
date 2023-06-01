using AdminLte.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminLte.Data.Configuration
{
    public class PayoutSettingConfiguration : IEntityTypeConfiguration<PayoutSetting>
    {
        public void Configure(EntityTypeBuilder<PayoutSetting> builder)
        {
            builder.Property(b => b.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAdd();
            builder.Property(b => b.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP").ValueGeneratedOnAddOrUpdate();

            builder.HasOne(b => b.PaymentMethod).WithMany().HasForeignKey(b => b.PaymentMethodId);
        }
    }
}
